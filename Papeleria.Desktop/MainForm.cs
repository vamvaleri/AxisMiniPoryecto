using System.Drawing.Drawing2D;
using Microsoft.EntityFrameworkCore;
using Papeleria.Core.Data;
using Papeleria.Core.Models;
using Papeleria.Core.Services;

namespace Papeleria.Desktop
{
    
    public class BotonAxis : Button
    {
        public int RadioBorde { get; set; } = 18;
        public Color ColorBorde { get; set; } = Color.FromArgb(74, 53, 37); 

        public BotonAxis()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(137, 196, 232); 
            ForeColor = Color.FromArgb(74, 53, 37);   
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using GraphicsPath path = ObtenerRutaRedondeada(rect, RadioBorde);
            this.Region = new Region(ObtenerRutaRedondeada(new Rectangle(0, 0, Width, Height), RadioBorde));

            using SolidBrush brush = new SolidBrush(BackColor);
            pevent.Graphics.FillPath(brush, path);

            using Pen pen = new Pen(ColorBorde, 1.5F);
            pevent.Graphics.DrawPath(pen, path);

            TextRenderer.DrawText(
                pevent.Graphics,
                Text,
                Font,
                rect,
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        private GraphicsPath ObtenerRutaRedondeada(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // Panel contenedor estilo Bento Card con bordes redondeados y línea fina café
    public class PanelBento : Panel
    {
        public int RadioBorde { get; set; } = 18;
        public Color ColorFondo { get; set; } = Color.White;
        public Color ColorBorde { get; set; } = Color.FromArgb(74, 53, 37);

        public PanelBento()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int d = RadioBorde * 2;
            using GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            using SolidBrush brush = new SolidBrush(ColorFondo);
            e.Graphics.FillPath(brush, path);

            using Pen pen = new Pen(ColorBorde, 1.5F);
            e.Graphics.DrawPath(pen, path);
        }
    }

    public class MainForm : Form
    {
        private readonly PapeleriaDbContext _context;
        private readonly InventarioVentasService _service;

       
        private readonly Color ColorCremaFondo = Color.FromArgb(250, 247, 242);    // #FAF7F2 
        private readonly Color ColorCelesteHielo = Color.FromArgb(227, 241, 250);  // #E3F1FA 
        private readonly Color ColorCelesteSuave = Color.FromArgb(203, 230, 247);  // #CBE6F7 
        private readonly Color ColorCelesteAcento = Color.FromArgb(137, 196, 232); // #89C4E8 
        private readonly Color ColorCafeEspresso = Color.FromArgb(74, 53, 37);     // #4A3525 
        private readonly Color ColorCafeMoca = Color.FromArgb(125, 90, 68);        // #7D5A44 

        private readonly Font FuenteMarca = new Font("Georgia", 20F, FontStyle.Bold);
        private readonly Font FuenteTitulos = new Font("Georgia", 15F, FontStyle.Bold);

        // Controles de Inventario
        private DataGridView dgvInventario = new();
        private TextBox txtCodigo = new(), txtNombre = new(), txtPrecio = new(), txtStock = new();
        private ComboBox cmbCategoria = new();

        // Controles de Ventas
        private TextBox txtCliente = new() { Text = "Cliente Mostrador" };
        private ComboBox cmbMetodoPago = new();
        private ComboBox cmbProductoVenta = new();
        private NumericUpDown numCantidadVenta = new() { Minimum = 1, Maximum = 1000, Value = 1 };
        private DataGridView dgvCarrito = new();
        private Label lblTotalVenta = new() { Text = "Total: $0", Font = new Font("Georgia", 15F, FontStyle.Bold) };
        private Dictionary<int, int> _carrito = new();

        // Controles de Historial
        private DataGridView dgvHistorial = new();

        public MainForm()
        {
            Text = "Axis — Gestión de Ventas e Inventario";
            Size = new Size(980, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ColorCremaFondo;
            ForeColor = ColorCafeEspresso;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "papeleria_ventas.db"));
            var options = new DbContextOptionsBuilder<PapeleriaDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            _context = new PapeleriaDbContext(options);
            _context.Database.EnsureCreated();
            _service = new InventarioVentasService(_context);

            ConstruirInterfaz();
            CargarDatos();
        }

        private void ConstruirInterfaz()
        {
            
            var panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };
            panelHeader.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Línea inferior café
                using Pen pen = new Pen(ColorCafeEspresso, 1.5F);
                e.Graphics.DrawLine(pen, 0, panelHeader.Height - 1, panelHeader.Width, panelHeader.Height - 1);

                // Destellos / asteriscos decorativos en celeste y café (estilo Image 1)
                DibujarAsterisco(e.Graphics, ColorCelesteAcento, panelHeader.Width - 65, 34, 14);
                DibujarEstrella(e.Graphics, ColorCafeMoca, panelHeader.Width - 115, 28, 9);
                DibujarAsterisco(e.Graphics, ColorCelesteSuave, panelHeader.Width - 155, 38, 10);
            };

            var lblLogo = new Label
            {
                Text = "✦ Axis",
                Font = FuenteMarca,
                ForeColor = ColorCafeEspresso,
                AutoSize = true,
                Location = new Point(22, 16)
            };

            var lblSubLogo = new Label
            {
                Text = "PUNTO DE VENTA & CONTROL DE INVENTARIO",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = ColorCafeMoca,
                AutoSize = true,
                Location = new Point(135, 27)
            };

            panelHeader.Controls.AddRange(new Control[] { lblLogo, lblSubLogo });

            var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(14, 6) };

            // ==========================================
            // PESTAÑA 1: INVENTARIO
            // ==========================================
            var tabInventario = new TabPage("▤ Inventario") { BackColor = ColorCremaFondo, Padding = new Padding(12) };

            var bannerInv = CrearBannerSeccion("INVENTARIO DE PRODUCTOS", "Catálogo activo y control de existencias");
            var contenedorFormInv = new Panel { Dock = DockStyle.Top, Height = 105, BackColor = ColorCremaFondo, Padding = new Padding(0, 8, 0, 8) };
            var cardFormInv = new PanelBento { Dock = DockStyle.Fill, ColorFondo = Color.White };

            txtCodigo.PlaceholderText = "Código (EJ-01)";
            txtCodigo.BackColor = ColorCremaFondo;
            txtCodigo.SetBounds(15, 14, 105, 25);

            txtNombre.PlaceholderText = "Nombre del producto";
            txtNombre.BackColor = ColorCremaFondo;
            txtNombre.SetBounds(130, 14, 230, 25);

            cmbCategoria.Items.AddRange(new[] { "Cuadernos", "Escritura", "Papeles", "Arte", "Archivo" });
            cmbCategoria.SelectedIndex = 0;
            cmbCategoria.BackColor = ColorCremaFondo;
            cmbCategoria.SetBounds(370, 14, 125, 25);

            txtPrecio.PlaceholderText = "Precio ($)";
            txtPrecio.BackColor = ColorCremaFondo;
            txtPrecio.SetBounds(505, 14, 100, 25);

            txtStock.PlaceholderText = "Stock";
            txtStock.BackColor = ColorCremaFondo;
            txtStock.SetBounds(615, 14, 70, 25);

            var btnAgregarProd = new BotonAxis { Text = "+ Guardar en Axis", BackColor = ColorCelesteAcento };
            btnAgregarProd.SetBounds(700, 11, 175, 32);
            btnAgregarProd.Click += BtnAgregarProd_Click;

            var btnReponer = new BotonAxis
            {
                Text = "✦ +10 Stock al Seleccionado",
                BackColor = ColorCafeEspresso,
                ForeColor = ColorCremaFondo
            };
            btnReponer.SetBounds(15, 48, 225, 30);
            btnReponer.Click += BtnReponer_Click;

            cardFormInv.Controls.AddRange(new Control[] { txtCodigo, txtNombre, cmbCategoria, txtPrecio, txtStock, btnAgregarProd, btnReponer });
            contenedorFormInv.Controls.Add(cardFormInv);

            EstilizarTablaTracker(dgvInventario);
            tabInventario.Controls.Add(dgvInventario);
            tabInventario.Controls.Add(contenedorFormInv);
            tabInventario.Controls.Add(bannerInv);

            // ==========================================
            // PESTAÑA 2: NUEVA VENTA
            // ==========================================
            var tabVentas = new TabPage("✦ Nueva Venta") { BackColor = ColorCremaFondo, Padding = new Padding(12) };

            var bannerVentas = CrearBannerSeccion("NUEVA VENTA", "Facturación rápida y descuento automático de stock");
            var contenedorVentaTop = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = ColorCremaFondo, Padding = new Padding(0, 8, 0, 8) };
            var cardVentaTop = new PanelBento { Dock = DockStyle.Fill, ColorFondo = Color.White };

            var lblCli = new Label { Text = "Cliente:", AutoSize = true, Location = new Point(15, 17), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = ColorCafeEspresso };
            txtCliente.BackColor = ColorCremaFondo;
            txtCliente.SetBounds(75, 14, 210, 25);

            var lblPago = new Label { Text = "Pago:", AutoSize = true, Location = new Point(305, 17), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = ColorCafeEspresso };
            cmbMetodoPago.Items.AddRange(new[] { "Efectivo", "Transferencia / Nequi", "Tarjeta" });
            cmbMetodoPago.SelectedIndex = 0;
            cmbMetodoPago.BackColor = ColorCremaFondo;
            cmbMetodoPago.SetBounds(355, 14, 170, 25);

            var lblProd = new Label { Text = "Producto:", AutoSize = true, Location = new Point(15, 55), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = ColorCafeEspresso };
            cmbProductoVenta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProductoVenta.BackColor = ColorCremaFondo;
            cmbProductoVenta.SetBounds(85, 52, 310, 25);

            var lblCant = new Label { Text = "Cant:", AutoSize = true, Location = new Point(410, 55), Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = ColorCafeEspresso };
            numCantidadVenta.BackColor = ColorCremaFondo;
            numCantidadVenta.SetBounds(450, 52, 75, 25);

            var btnAgregarCarrito = new BotonAxis { Text = "+ Agregar a Factura", BackColor = ColorCelesteAcento };
            btnAgregarCarrito.SetBounds(545, 49, 175, 32);
            btnAgregarCarrito.Click += BtnAgregarCarrito_Click;

            cardVentaTop.Controls.AddRange(new Control[] { lblCli, txtCliente, lblPago, cmbMetodoPago, lblProd, cmbProductoVenta, lblCant, numCantidadVenta, btnAgregarCarrito });
            contenedorVentaTop.Controls.Add(cardVentaTop);

            var contenedorVentaBottom = new Panel { Dock = DockStyle.Bottom, Height = 72, BackColor = ColorCremaFondo, Padding = new Padding(0, 8, 0, 0) };
            var cardVentaBottom = new PanelBento { Dock = DockStyle.Fill, ColorFondo = ColorCelesteHielo };

            lblTotalVenta.Location = new Point(20, 16);
            lblTotalVenta.ForeColor = ColorCafeEspresso;
            lblTotalVenta.AutoSize = true;

            var btnConfirmarVenta = new BotonAxis
            {
                Text = "✦ Confirmar y Cobrar",
                BackColor = ColorCafeEspresso,
                ForeColor = ColorCremaFondo,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnConfirmarVenta.SetBounds(690, 11, 210, 38);
            btnConfirmarVenta.Click += BtnConfirmarVenta_Click;

            cardVentaBottom.Controls.AddRange(new Control[] { lblTotalVenta, btnConfirmarVenta });
            contenedorVentaBottom.Controls.Add(cardVentaBottom);

            EstilizarTablaTracker(dgvCarrito);
            tabVentas.Controls.Add(dgvCarrito);
            tabVentas.Controls.Add(contenedorVentaTop);
            tabVentas.Controls.Add(contenedorVentaBottom);
            tabVentas.Controls.Add(bannerVentas);

            // ==========================================
            // PESTAÑA 3: HISTORIAL DE VENTAS
            // ==========================================
            var tabHistorial = new TabPage("☷ Historial de Ventas") { BackColor = ColorCremaFondo, Padding = new Padding(12) };
            var bannerHist = CrearBannerSeccion("HISTORIAL DE VENTAS", "Registro histórico de transacciones en Axis");

            EstilizarTablaTracker(dgvHistorial);
            tabHistorial.Controls.Add(dgvHistorial);
            tabHistorial.Controls.Add(bannerHist);

            tabs.TabPages.AddRange(new[] { tabInventario, tabVentas, tabHistorial });
            Controls.Add(tabs);
            Controls.Add(panelHeader);
        }

        private PanelBento CrearBannerSeccion(string titulo, string subtitulo)
        {
            var banner = new PanelBento
            {
                Dock = DockStyle.Top,
                Height = 64,
                ColorFondo = ColorCelesteSuave,
                Padding = new Padding(18, 8, 18, 8)
            };

            banner.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                DibujarAsterisco(e.Graphics, Color.White, banner.Width - 45, 32, 13);
                DibujarEstrella(e.Graphics, ColorCafeMoca, banner.Width - 85, 32, 8);
            };

            var lblTit = new Label
            {
                Text = titulo,
                Font = FuenteTitulos,
                ForeColor = ColorCafeEspresso,
                AutoSize = true,
                Location = new Point(18, 9),
                BackColor = Color.Transparent
            };

            var lblSub = new Label
            {
                Text = subtitulo,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = ColorCafeMoca,
                AutoSize = true,
                Location = new Point(20, 38),
                BackColor = Color.Transparent
            };

            banner.Controls.AddRange(new Control[] { lblTit, lblSub });
            return banner;
        }

       
        private void EstilizarTablaTracker(DataGridView dgv)
        {
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgv.BackgroundColor = ColorCremaFondo;
            dgv.GridColor = ColorCafeEspresso;

            dgv.DefaultCellStyle.BackColor = ColorCelesteHielo;
            dgv.DefaultCellStyle.ForeColor = ColorCafeEspresso;
            dgv.DefaultCellStyle.SelectionBackColor = ColorCelesteAcento;
            dgv.DefaultCellStyle.SelectionForeColor = ColorCafeEspresso;
            dgv.RowsDefaultCellStyle.BackColor = ColorCelesteHielo;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorCelesteSuave;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorCafeEspresso;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 36;
            dgv.RowTemplate.Height = 30;
        }

        private void DibujarAsterisco(Graphics g, Color color, int cx, int cy, int radio)
        {
            using Pen pen = new Pen(color, 2.2F) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            for (int i = 0; i < 4; i++)
            {
                double ang = i * Math.PI / 4.0;
                int dx = (int)(Math.Cos(ang) * radio);
                int dy = (int)(Math.Sin(ang) * radio);
                g.DrawLine(pen, cx - dx, cy - dy, cx + dx, cy + dy);
            }
        }

        private void DibujarEstrella(Graphics g, Color color, int cx, int cy, int radio)
        {
            PointF[] pts = new PointF[10];
            for (int i = 0; i < 10; i++)
            {
                double r = (i % 2 == 0) ? radio : radio * 0.45;
                double ang = -Math.PI / 2 + i * Math.PI / 5.0;
                pts[i] = new PointF(cx + (float)(Math.Cos(ang) * r), cy + (float)(Math.Sin(ang) * r));
            }
            using SolidBrush brush = new SolidBrush(color);
            g.FillPolygon(brush, pts);
        }

        private void CargarDatos()
        {
            var productos = _context.Productos.ToList();
            dgvInventario.DataSource = productos.Select(p => new
            {
                p.Id,
                p.Codigo,
                p.Nombre,
                p.Categoria,
                Precio = $"${p.PrecioVenta:N0}",
                p.Stock,
                Estado = p.EstadoInventario()
            }).ToList();

            cmbProductoVenta.DataSource = productos.Where(p => p.Stock > 0).ToList();
            cmbProductoVenta.DisplayMember = "Nombre";
            cmbProductoVenta.ValueMember = "Id";

            dgvHistorial.DataSource = _context.Ventas
                .OrderByDescending(v => v.Fecha)
                .Select(v => new
                {
                    Factura = v.Id,
                    Fecha = v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Cliente = v.NombreClienteMostrador,
                    Pago = v.MetodoPago,
                    Total = $"${v.TotalVenta:N0}"
                }).ToList();
        }

        private void BtnAgregarProd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) ||
                !decimal.TryParse(txtPrecio.Text, out decimal precio) || !int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("Completa todos los campos con valores válidos.", "Axis — Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _context.Productos.Add(new Producto
            {
                Codigo = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Categoria = cmbCategoria.Text,
                PrecioVenta = precio,
                Stock = stock,
                StockMinimo = 5
            });
            _context.SaveChanges();

            txtCodigo.Clear(); txtNombre.Clear(); txtPrecio.Clear(); txtStock.Clear();
            CargarDatos();
            MessageBox.Show("Producto registrado en Axis.", "Axis — Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReponer_Click(object? sender, EventArgs e)
        {
            if (dgvInventario.CurrentRow == null) return;
            int id = (int)dgvInventario.CurrentRow.Cells["Id"].Value;
            var prod = _context.Productos.Find(id);
            if (prod != null)
            {
                prod.ReponerStock(10);
                _context.SaveChanges();
                CargarDatos();
            }
        }

        private void BtnAgregarCarrito_Click(object? sender, EventArgs e)
        {
            if (cmbProductoVenta.SelectedItem is not Producto prod) return;
            int cant = (int)numCantidadVenta.Value;

            int cantidadAcumulada = _carrito.ContainsKey(prod.Id) ? _carrito[prod.Id] + cant : cant;
            if (!prod.TieneStockDisponible(cantidadAcumulada))
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {prod.Stock}", "Axis — Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _carrito[prod.Id] = cantidadAcumulada;
            ActualizarTablaCarrito();
        }

        private void ActualizarTablaCarrito()
        {
            var filas = new List<object>();
            decimal total = 0;

            foreach (var item in _carrito)
            {
                var p = _context.Productos.Find(item.Key);
                if (p != null)
                {
                    decimal sub = p.PrecioVenta * item.Value;
                    total += sub;
                    filas.Add(new { p.Codigo, p.Nombre, Cantidad = item.Value, Unitario = $"${p.PrecioVenta:N0}", Subtotal = $"${sub:N0}" });
                }
            }

            dgvCarrito.DataSource = filas;
            lblTotalVenta.Text = $"Total: ${total:N0}";
        }

        private async void BtnConfirmarVenta_Click(object? sender, EventArgs e)
        {
            var res = await _service.RegistrarVentaAsync(txtCliente.Text, cmbMetodoPago.Text, _carrito);
            if (!res.Exito)
            {
                MessageBox.Show(res.Mensaje, "Axis — Error en Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(res.Mensaje, "Axis — Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _carrito.Clear();
            ActualizarTablaCarrito();
            CargarDatos();
        }
    }
}
