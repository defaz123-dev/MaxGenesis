using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Max;

namespace MaxGenesis
{
    public static class PluginMain
    {
        private static GenesisUI _ui;
        private static ApiConnector _api;
        private static MeshManager _mesh;
        private static MaterialManager _material;
        private static IGlobal _global;
        private static IINode _lastGeneratedNode;

        private class MaxWin32Window : IWin32Window
        {
            public IntPtr Handle { get; }
            public MaxWin32Window(IntPtr handle) { Handle = handle; }
        }

        public static void Show()
        {
            if (_global == null)
            {
                _global = GlobalInterface.Instance;
                // Ajustar la URL a tu servidor de Python
                _api = new ApiConnector("http://localhost:5000/generate");
                _mesh = new MeshManager(_global);
                _material = new MaterialManager(_global);
            }

            if (_ui == null || _ui.IsDisposed)
            {
                _ui = new GenesisUI();
                _ui.OnGenerateRequested += HandleGenerate;
                _ui.OnRotateRequested += HandleRotate;
                _ui.OnColorChanged += HandleColorChange;
            }

            _ui.Show(new MaxWin32Window(ManagedServices.AppSDK.GetMaxHWND()));
        }

        private static async void HandleGenerate(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Por favor, selecciona una imagen primero.", "MaxGenesis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _ui.SetStatus("Generando 3D con IA... espera un momento.", Color.SkyBlue);
                
                // 1. Llamar a la API de Python
                string objPath = await _api.Generate3DFromImage(imagePath);

                // 2. Importar el modelo a 3ds Max
                _lastGeneratedNode = _mesh.ImportModel(objPath);

                if (_lastGeneratedNode != null)
                {
                    // 3. Aplicar material base
                    _material.ApplyMaterial(_lastGeneratedNode, Color.LightGray);
                    
                    _ui.SetStatus("¡Modelo generado e importado con éxito!", Color.LimeGreen);
                }
                else
                {
                    _ui.SetStatus("Error al importar el modelo.", Color.OrangeRed);
                }
            }
            catch (Exception ex)
            {
                _ui.SetStatus("Error: " + ex.Message, Color.Red);
                MessageBox.Show("Error durante la generación AI: " + ex.Message);
            }
        }

        private static void HandleRotate()
        {
            if (_lastGeneratedNode == null) return;
            _mesh.RotateNode360(_lastGeneratedNode, 45.0f);
        }

        private static void HandleColorChange(Color color)
        {
            if (_lastGeneratedNode == null) return;
            _material.UpdateColor(_lastGeneratedNode, color);
        }
    }
}
