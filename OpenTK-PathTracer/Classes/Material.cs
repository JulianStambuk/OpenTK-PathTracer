﻿using System;

using OpenTK;

namespace OpenTK_PathTracer
{
    class Material : UBOCompatible
    {
        public static Material Zero => new Material(albedo: Vector3.One, emissiv: Vector3.Zero, refractionColor: Vector3.Zero, specularChance: 0.0f, specularRoughness: 0.0f, indexOfRefraction: 1.0f, refractionChance: 0.0f, refractionRoughnes: 0.0f);
        public static readonly int GPUInstanceSize = Vector4.SizeInBytes * 4;

        public Vector3 Albedo;
        public Vector3 Emissiv;
        public Vector3 RefractionColor;
        public float SpecularChance;
        public float SpecularRoughness;
        public float IOR;
        public float RefractionChance;
        public float RefractionRoughnes;
        public Material(Vector3 albedo, Vector3 emissiv, Vector3 refractionColor, float specularChance, float specularRoughness, float indexOfRefraction, float refractionChance, float refractionRoughnes)
        {
            // Note: diffuse chance is 1.0f - (SpecularChance + RefractionChance). So must add up to 1.0

            Albedo = albedo;
            Emissiv = emissiv;
            RefractionColor = refractionColor;
            SpecularChance = Math.Clamp(specularChance, 0, 1.0f - RefractionChance);
            SpecularRoughness = specularRoughness;
            IOR = Math.Max(indexOfRefraction, 1.0f);
            RefractionChance = Math.Clamp(refractionChance, 0, 1.0f - SpecularChance);
            RefractionRoughnes = refractionRoughnes;
        }

        public override int BufferOffset => throw new NotImplementedException();

        readonly Vector4[] GPUData = new Vector4[4];
        public override Vector4[] GetGPUFriendlyData()
        {
            GPUData[0].Xyz = Albedo;
            GPUData[0].W = SpecularChance;
           
            GPUData[1].Xyz = Emissiv;
            GPUData[1].W = SpecularRoughness;

            GPUData[2].Xyz = RefractionColor;
            GPUData[2].W = RefractionChance;

            GPUData[3].X = RefractionRoughnes;
            GPUData[3].Y = IOR;
            return GPUData;
        }

        public override string ToString()
        {
            return $"<Material A: {Albedo},\n E: {Emissiv},\n R: {RefractionColor},\n S%: {SpecularChance},\n SR: {SpecularRoughness},\n IOR: {IOR},\n R% {RefractionChance},\n RR: {RefractionRoughnes}>";
        }


        private readonly static Random rnd = new Random();
        public static Material GetRndMaterial()
        {
            bool isEmissiv = rnd.NextDouble() < 0.2;
            return new Material(albedo: RndVector3(), emissiv: isEmissiv ? RndVector3() : Vector3.Zero, refractionColor: RndVector3() * 2, specularChance: (float)rnd.NextDouble(), specularRoughness: (float)rnd.NextDouble(), indexOfRefraction: (float)rnd.NextDouble() + 1, refractionChance: (float)rnd.NextDouble(), refractionRoughnes: (float)rnd.NextDouble());
        }

        private static Vector3 RndVector3() => new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
    }
}