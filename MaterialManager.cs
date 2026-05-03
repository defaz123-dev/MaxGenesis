using System;
using System.Drawing;
using Autodesk.Max;

namespace MaxGenesis
{
    public class MaterialManager
    {
        private readonly IGlobal _global;
        private readonly IInterface _ip;

        public MaterialManager(IGlobal global)
        {
            _global = global;
            _ip = global.COREInterface;
        }

        public void ApplyMaterial(IINode node, Color color)
        {
            if (node == null) return;

            // Crear un Physical Material (Clase ID 0x38ba4ce1, 0x5d0f3c25)
            // Para simplicidad, usaremos el Standard Material si el Physical no es accesible directamente
            IClass_ID cid = _global.Class_ID.Create(2, 0); // Standard Material
            IMtl mat = _ip.CreateInstance(SClass_ID.Material, cid) as IMtl;

            if (mat != null)
            {
                // Configurar el color Diffuse
                IColor maxCol = _global.Color.Create(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
                // El parámetro 1 suele ser Diffuse en Standard
                mat.SetAmbient(maxCol, 0);
                mat.SetDiffuse(maxCol, 0);
                
                node.Mtl = mat;
            }
        }

        public void UpdateColor(IINode node, Color color)
        {
            if (node == null || node.Mtl == null) return;
            IColor maxCol = _global.Color.Create(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            node.Mtl.SetDiffuse(maxCol, _ip.Time);
        }
    }
}
