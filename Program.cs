
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace paint
{
partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }

 public partial class Form1 : Form
    {
        private const int GRID_SIZE = 32;
        private const int PIXEL_SIZE = 10;

        private Color[] colors =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 170),
            Color.FromArgb(0, 170, 0),
            Color.FromArgb(0, 170, 170),
            Color.FromArgb(170, 0, 0),
            Color.FromArgb(170, 0, 170),
            Color.FromArgb(170, 85, 0),
            Color.FromArgb(170, 170, 170),

            Color.FromArgb(85, 85, 85),
            Color.FromArgb(85, 85, 255),
            Color.FromArgb(85, 255, 85),
            Color.FromArgb(85, 255, 255),
            Color.FromArgb(255, 85, 85),
            Color.FromArgb(255, 85, 255),
            Color.FromArgb(255, 255, 85),
            Color.FromArgb(255, 255, 255)
        };

        private Color currentColor;

        private Color[,] grid =
            new Color[GRID_SIZE, GRID_SIZE];

        private Panel canvas;
        private FlowLayoutPanel palette;

        public Form1()
        {
            Text = "32x32 VGA Pixel Painter";
            Size = new Size(420, 640);
            StartPosition = FormStartPosition.CenterScreen;
            
            BackColor = Color.FromArgb(255, 255, 255);
            ForeColor = Color.White;

            Font = new Font("Arial", 10);

            currentColor = colors[0];

            // Initialize drawing grid
            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    grid[x, y] = colors[0];
                }
            }

            InitializeUI();
            DrawCanvas();
        }

        private void InitializeUI()
        {
            Label title = new Label();

            title.Text = "32x32 VGA Painter";
            title.Font = new Font("Arial", 15);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Location = new Point(0, 15);
            title.Size = new Size(400, 35);

            Controls.Add(title);

            // Color palette
            palette = new FlowLayoutPanel();

            palette.Location = new Point(65, 65);
            palette.Size = new Size(290, 75);
            palette.BackColor = BackColor;

            palette.WrapContents = true;
            palette.AutoScroll = false;

            Controls.Add(palette);

            foreach (Color c in colors)
            {
                Button colorButton = new Button();

                colorButton.Size = new Size(30, 30);
                colorButton.Margin = new Padding(2, 2, 3, 3);

                colorButton.BackColor = c;
                colorButton.FlatStyle = FlatStyle.Flat;
                colorButton.FlatAppearance.BorderColor = Color.Black;
                colorButton.FlatAppearance.BorderSize = 2;

                colorButton.Click += (s, e) =>
                {
                    currentColor = c;
                };

                palette.Controls.Add(colorButton);
            }

            // Drawing canvas
            canvas = new Panel();

            canvas.Location = new Point(48, 155);
            canvas.Size = new Size(324, 324);
            canvas.BackColor = Color.Black;
            
            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;

            Controls.Add(canvas);

            // Download CSV button
            Button saveButton = new Button();

            saveButton.Text = "Download CSV";
            saveButton.Location = new Point(75, 490);
            saveButton.Size = new Size(120, 30);
            saveButton.BackColor = Color.White;
            saveButton.ForeColor = Color.Black;
            saveButton.Click += (s, e) =>
            {
                SaveFile();
            };

            Controls.Add(saveButton);

            // Load CSV button
            Button loadButton = new Button();

            loadButton.Text = "Load CSV";
            loadButton.Location = new Point(205, 490);
            loadButton.Size = new Size(120, 30);
            loadButton.BackColor = Color.White;
            loadButton.ForeColor = Color.Black;
            loadButton.Click += (s, e) =>
            {
                LoadFile();
            };

            Controls.Add(loadButton);
        }

        // Draw all pixels
        private void DrawCanvas()
        {
            canvas.Invalidate();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.Black);

            for (int y = 0; y < GRID_SIZE; y++)
            {
                for (int x = 0; x < GRID_SIZE; x++)
                {
                    using (SolidBrush brush =
                        new SolidBrush(grid[x, y]))
                    {
                        g.FillRectangle(
                            brush,
                            x * PIXEL_SIZE + 2,
                            y * PIXEL_SIZE + 2,
                            PIXEL_SIZE,
                            PIXEL_SIZE
                        );
                    }
                }
            }

            // White canvas border
            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(
                    pen, 1, 1,
                    321, 321
                );
            }
        }

        // Paint one pixel
        private void PaintPixel(int mouseX, int mouseY)
        {
            int x = (mouseX - 2) / PIXEL_SIZE;
            int y = (mouseY - 2) / PIXEL_SIZE;

            if (x >= 0 && x < GRID_SIZE &&
                y >= 0 && y < GRID_SIZE)
            {
                grid[x, y] = currentColor;
                DrawCanvas();
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PaintPixel(e.X, e.Y);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PaintPixel(e.X, e.Y);
            }
        }

        // Export CSV
        private void SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "CSV files (*.csv)|*.csv";
            dialog.FileName = "art.csv";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer =
                    new StreamWriter(dialog.FileName))
                {
                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        string[] row = new string[GRID_SIZE];

                        for (int x = 0; x < GRID_SIZE; x++)
                        {
                            Color c = grid[x, y];

                            row[x] = String.Format(
                                "#{0:X2}{1:X2}{2:X2}",
                                c.R, c.G, c.B
                            );
                        }

                        writer.WriteLine(
                            String.Join(",", row)
                        );
                    }
                }
            }
        }

        // Import CSV
        private void LoadFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "CSV files (*.csv)|*.csv";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] rows =
                        File.ReadAllLines(dialog.FileName);

                    if (rows.Length != GRID_SIZE)
                        throw new Exception("Invalid number of rows.");

                    Color[,] newGrid =
                        new Color[GRID_SIZE, GRID_SIZE];

                    for (int y = 0; y < GRID_SIZE; y++)
                    {
                        string[] values = rows[y].Split(',');

                        if (values.Length != GRID_SIZE)
                            throw new Exception("Invalid number of columns.");

                        for (int x = 0; x < GRID_SIZE; x++)
                        {
                            Color c = ColorTranslator.FromHtml(
                                values[x].Trim()
                            );

                            newGrid[x, y] = c;
                        }
                    }

                    grid = newGrid;
                    DrawCanvas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error loading CSV:\n" + ex.Message
                    );
                }
            }
        }
    }


    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }
    }
}
