using System;
using System.Drawing;
using System.Windows.Forms;
using MaxCustomControls;

namespace MaxGenesis
{
    public class GenesisUI : MaxForm
    {
        private Label _lblStatus;
        private PictureBox _pbPreview;
        private Button _btnSelectImage;
        private Button _btnGenerate;
        private Button _btnRotate;
        private Button _btnPickColor;
        
        public event Action<string> OnGenerateRequested;
        public event Action OnRotateRequested;
        public event Action<Color> OnColorChanged;

        private string _selectedImagePath;

        public GenesisUI()
        {
            InitializeComponents();
            this.Text = "MaxGenesis AI v1.0";
            this.Size = new Size(360, 600);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void InitializeComponents()
        {
            Panel mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            Label header = new Label {
                Text = "🤖 MaxGenesis AI",
                Dock = DockStyle.Top, Height = 40,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(144, 238, 144),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // PASO 1: IMAGEN
            GroupBox gp1 = CreateGroupBox("1. CARGAR SEMILLA (IMAGEN)", 200);
            _pbPreview = new PictureBox {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            _btnSelectImage = CreateButton("CARGAR IMAGEN", DockStyle.Bottom, Color.FromArgb(52, 152, 219));
            _btnSelectImage.Click += SelectImage_Click;
            gp1.Controls.Add(_pbPreview);
            gp1.Controls.Add(_btnSelectImage);

            // PASO 2: GENERAR
            _btnGenerate = new Button {
                Text = "🚀 GENERAR MODELO 3D",
                Dock = DockStyle.Top, Height = 60,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(0, 20, 0, 20)
            };
            _btnGenerate.Click += (s, e) => OnGenerateRequested?.Invoke(_selectedImagePath);

            // PASO 3: PULIDO
            GroupBox gp3 = CreateGroupBox("3. PULIDO Y ACABADO", 150);
            _btnRotate = CreateButton("GIRAR 360°", DockStyle.Top, Color.FromArgb(155, 89, 182));
            _btnRotate.Click += (s, e) => OnRotateRequested?.Invoke();

            _btnPickColor = CreateButton("CAMBIAR COLOR", DockStyle.Top, Color.FromArgb(230, 126, 34));
            _btnPickColor.Click += PickColor_Click;

            gp3.Controls.Add(_btnPickColor);
            gp3.Controls.Add(_btnRotate);

            _lblStatus = new Label {
                Text = "Listo para empezar.",
                Dock = DockStyle.Bottom, Height = 30,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };

            mainPanel.Controls.Add(gp3);
            mainPanel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            mainPanel.Controls.Add(_btnGenerate);
            mainPanel.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            mainPanel.Controls.Add(gp1);
            mainPanel.Controls.Add(header);
            
            this.Controls.Add(mainPanel);
            this.Controls.Add(_lblStatus);
        }

        private void SelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _selectedImagePath = ofd.FileName;
                    _pbPreview.Image = Image.FromFile(_selectedImagePath);
                    _lblStatus.Text = "Imagen cargada: " + System.IO.Path.GetFileName(_selectedImagePath);
                }
            }
        }

        private void PickColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    OnColorChanged?.Invoke(cd.Color);
                }
            }
        }

        public void SetStatus(string text, Color color)
        {
            _lblStatus.Text = text;
            _lblStatus.ForeColor = color;
        }

        private GroupBox CreateGroupBox(string text, int h)
        {
            return new GroupBox {
                Text = text, Dock = DockStyle.Top, Height = h,
                ForeColor = Color.FromArgb(144, 238, 144),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding = new Padding(10)
            };
        }

        private Button CreateButton(string text, DockStyle dock, Color backColor)
        {
            return new Button {
                Text = text, Dock = dock, Height = 35,
                BackColor = backColor, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
    }
}
