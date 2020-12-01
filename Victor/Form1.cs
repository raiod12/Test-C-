using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Victor
{
    public partial class Form1 : Form
    {

        public Form1()
        {

            if (!File.Exists(@"AcmeDatabase.sqlite")) {
                SQLiteConnection.CreateFile("AcmeDatabase.sqlite");
                SQLiteConnection conn = new SQLiteConnection("Data Source=AcmeDatabase.sqlite;Version=3;");
                conn.Open();
                string sql = "CREATE TABLE DB_VOO (ID_VOO INTEGER PRIMARY KEY AUTOINCREMENT, DATA_VOO DATETIME NOT NULL, CUSTO NUMERIC (10, 2) NOT NULL,DISTANCIA INT NOT NULL, CAPTURA CHAR (1)NOT NULL, NIVEL_DOR INT NOT NULL);";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();

            }

            InitializeComponent();
        }
        SQLiteConnection conn = new SQLiteConnection("Data Source=AcmeDatabase.sqlite;Version=3;");
        private static SQLiteConnection sqliteConnection;
        private static SQLiteConnection DbConnection()
        {
            sqliteConnection = new SQLiteConnection("Data Source=AcmeDatabase.sqlite;Version=3;");
            sqliteConnection.Open();
            return sqliteConnection;
        }
        public void GridUpdate()
        {
            conn.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT ID_VOO, DATA_VOO, char(CAPTURA), NIVEL_DOR FROM DB_VOO", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            this.dataGridView1.Columns["ID_VOO"].Visible = false;
            this.dataGridView1.Columns[2].HeaderText = "CAPTURA";
            conn.Close();
        }

        public void ClearFields()
        {
            dateTimeData.Value = DateTime.Now;
            txtCusto.Text = "";
            txtDistancia.Text = "";
            radioBtnSim.Checked = true;
            cbDor.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GridUpdate();
            radioBtnSim.Checked = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSalvar.Enabled = true;
            btnCancelar.Enabled = true;

            int idConsulta = Int32.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
            string sqlcmd = "SELECT ID_VOO, DATA_VOO, CUSTO, DISTANCIA, CAPTURA, NIVEL_DOR FROM DB_VOO WHERE ID_VOO=" + idConsulta;
            DataTable dt = new DataTable();

            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = sqlcmd;
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        dateTimeData.Value = DateTime.Parse(row["DATA_VOO"].ToString());
                        txtCusto.Text = row["CUSTO"].ToString();
                        txtDistancia.Text = row["DISTANCIA"].ToString();
                        if (row["CAPTURA"].Equals("S"))
                        {
                            radioBtnSim.Checked = true;
                        }
                        else
                        {
                            radioBtnNao.Checked = true;
                        }
                        cbDor.Text = row["NIVEL_DOR"].ToString();
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Erro ao preencher formulário com informações do GridView \n" + ex);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {

            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    int idConsulta = Int32.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    cmd.CommandText = "DELETE FROM DB_VOO Where ID_VOO=@Id";
                    cmd.Parameters.AddWithValue("@Id", idConsulta);
                    cmd.ExecuteNonQuery();
                    GridUpdate();
                    ClearFields();
                    MessageBox.Show("Registro deletado com sucesso!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {

            if (txtCusto.Text != "" || txtDistancia.Text != "" || cbDor.Text != "")
            {
                Voo voo = new Voo();
                voo.DATA_VOO = dateTimeData.Value;
                voo.CUSTO = Double.Parse(txtCusto.Text);
                voo.DISTANCIA = Int32.Parse(txtDistancia.Text);
                if (radioBtnSim.Checked)
                {
                    voo.CAPTURA = Char.Parse("S");
                }
                else
                {
                    voo.CAPTURA = Char.Parse("N");
                }
                voo.NIVEL_DOR = Int32.Parse(cbDor.Text);

                conn.Open();
                try
                {
                    SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO DB_VOO (DATA_VOO, CUSTO, DISTANCIA, CAPTURA, NIVEL_DOR) VALUES (@DATA_VOO,@CUSTO,@DISTANCIA,@CAPTURA,@NIVEL_DOR)", conn);
                    insertSQL.Parameters.AddWithValue("@DATA_VOO", voo.DATA_VOO);
                    insertSQL.Parameters.AddWithValue("@CUSTO", voo.CUSTO);
                    insertSQL.Parameters.AddWithValue("@DISTANCIA", voo.DISTANCIA);
                    insertSQL.Parameters.AddWithValue("@CAPTURA", voo.CAPTURA);
                    insertSQL.Parameters.AddWithValue("@NIVEL_DOR", voo.NIVEL_DOR);
                    insertSQL.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Falha na inclusão do registro /n" + ex.ToString());
                }
                conn.Close();
                GridUpdate();
                ClearFields();
                MessageBox.Show("Registro incluido com sucesso!");
            }else{
                MessageBox.Show("Favor não deixar nenhum campo vazio");
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCusto.Text != "" || txtDistancia.Text != "" || cbDor.Text != "") {
                    using (var cmd = new SQLiteCommand(DbConnection()))
                    {
                        
                        int idConsulta;
                        idConsulta = Int32.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                        cmd.CommandText = "UPDATE DB_VOO SET DATA_VOO=@DATA, CUSTO=@CUSTO, DISTANCIA=@DISTANCIA, CAPTURA=@CAPTURA, NIVEL_DOR=@NIVEL_DOR WHERE ID_VOO=@ID";
                        cmd.Parameters.AddWithValue("@ID", idConsulta);
                        cmd.Parameters.AddWithValue("@DATA", DateTime.Parse(dateTimeData.Value.ToString()));
                        cmd.Parameters.AddWithValue("@CUSTO", txtCusto.ToString());
                        cmd.Parameters.AddWithValue("@DISTANCIA", txtDistancia.ToString());
                        //Verifica o valor de captura
                        if (radioBtnSim.Checked)
                        {
                            cmd.Parameters.AddWithValue("@CAPTURA", "S");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CAPTURA", "N");
                        }
                        cmd.Parameters.AddWithValue("@NIVEL_DOR", cbDor.ToString());
                        cmd.ExecuteNonQuery();
                    }else
                    {
                        MessageBox.Show("Favor não deixar nenhum campo em branco.");
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar registro /n"+ex);
            }
            MessageBox.Show("Registro atualizado com sucesso!");
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
            GridUpdate();
            ClearFields();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnCancelar.Enabled = false;
            btnSalvar.Enabled = false;
            ClearFields();
            GridUpdate();
        }

        
    }
}
