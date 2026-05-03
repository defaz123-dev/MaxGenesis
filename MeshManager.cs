using System;
using Autodesk.Max;

namespace MaxGenesis
{
    public class MeshManager
    {
        private readonly IGlobal _global;
        private readonly IInterface _ip;

        public MeshManager(IGlobal global)
        {
            _global = global;
            _ip = global.COREInterface;
        }

        public IINode ImportModel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            // Importar el archivo OBJ
            // Nota: ImportFromFile devuelve true/false. El nodo importado suele quedar seleccionado.
            bool success = _ip.ImportFromFile(filePath, false, null);
            
            if (success && _ip.SelNodeCount > 0)
            {
                return _ip.GetSelNode(0);
            }
            return null;
        }

        public void RotateNode360(IINode node, float angleDegrees)
        {
            if (node == null) return;
            float rad = angleDegrees * (float)Math.PI / 180.0f;
            IMatrix3 tm = node.GetNodeTM(_ip.Time, null);
            tm.PreRotateZ(rad);
            node.SetNodeTM(_ip.Time, tm);
        }
    }
}
