using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace DB_Train_
{
    public partial class Form1 : Form
    {

        static string conString = "Server=localhost;Database=peopledb;Uid=root;pwd=;";
        MySqlConnection con = new MySqlConnection(conString);
        MySqlCommand cmd;
        MySqlDataAdapter adapter;
        DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();

            //DataGridView PROPERTIES

            dataGridViewInfo.ColumnCount = 4;
            dataGridViewInfo.Columns[0].Name = "ID";
            dataGridViewInfo.Columns[1].Name = "Name";
            dataGridViewInfo.Columns[2].Name = "Position";
            dataGridViewInfo.Columns[3].Name = "Team";

            dataGridViewInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //SELECTION MODE
            dataGridViewInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewInfo.MultiSelect = false;
        }

        //INSERT INTO DB
        private void add(string name, string pos, string team)
        {
            //SQT STATEMENT
            string sql = "INSERT INTO peopletb(Name,Position,Team) VALUES(@NAME,@POSITION,@TEAM)";
            cmd = new MySqlCommand(sql, con);

            //ADD PARAMETERS
            cmd.Parameters.AddWithValue("@NAME", name);
            cmd.Parameters.AddWithValue("@POSITION", pos);
            cmd.Parameters.AddWithValue("@TEAM", team);

            //OPEN AND EXEC INSERT
            try
            {
                con.Open();

                if (cmd.ExecuteNonQuery() > 0)
                {
                    cleartxts();
                    MessageBox.Show("Successfully Inserted!");
                }

                con.Close();

                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //ADD TO DGVIEW
        private void populate(String id,String name, string pos, string team)
        {
            dataGridViewInfo.Rows.Add(id, name, pos, team);
        }

        //RETRIEVE FROM DB
        private void retrieve()
        {
            dataGridViewInfo.Rows.Clear();

            //SQL STATEMENT
            string sql = "SELECT * FROM peopleTB";
            cmd = new MySqlCommand(sql, con);

            //OPEN CON,RETRIEVE,FILL DGVIEW
            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(dt);

                //LOOP THROUGH DT
                foreach(DataRow row in dt.Rows)
                {
                    populate(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString());
                }
                con.Close();

                //CLEAR DT
                dt.Rows.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //UPDATE DB
        private void update(int id, string name, string pos, string team)
        {
            //SQL STATEMENT
            string sql = "UPDATE peopleTB SET Name='" + name + "', Position ='" + pos + "', Team='" + team + "' WHERE ID=" + id + "";
            cmd = new MySqlCommand(sql, con);

            //OPEN CON,UPDATE,RETRIEVE DGVIEW

            try
            {
                con.Open();
                adapter = new MySqlDataAdapter(cmd);

                adapter.UpdateCommand = con.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;

                if(adapter.UpdateCommand.ExecuteNonQuery()>0)
                {
                    cleartxts();
                    MessageBox.Show("Successfully Updated!");
                }

                con.Close();

                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //DELETE FROM DB
        private void delete(int id)
        {
            //SQL STATEMENT
            string sql = "DELETE FROM peopleTB WHERE ID=" + id + "";
            cmd = new MySqlCommand(sql, con);

            //OPEN CON, DELETE, CLOSE CON
            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.DeleteCommand = con.CreateCommand();

                adapter.DeleteCommand.CommandText = sql;

                //PROMPT FOR CONFIRMATION
                if(MessageBox.Show("Sure??","DELETE",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning)==DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        cleartxts();
                        MessageBox.Show("Successfully Deleted!");
                    }
                }

                con.Close();

                retrieve();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        //CLEAR TEXT
        private void cleartxts()
        {
            txtname.Text = "";
            txtposition.Text = "";
            txtteam.Text = "";
        }

        private void dataGridViewInfo_MouseClick(object sender, MouseEventArgs e)
        {
            txtname.Text = dataGridViewInfo.SelectedRows[0].Cells[1].Value.ToString();
            txtposition.Text = dataGridViewInfo.SelectedRows[0].Cells[2].Value.ToString();
            txtteam.Text = dataGridViewInfo.SelectedRows[0].Cells[3].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            add(txtname.Text, txtposition.Text, txtteam.Text);
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            retrieve();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string selected = dataGridViewInfo.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);

            update(id, txtname.Text, txtposition.Text, txtteam.Text);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string selected = dataGridViewInfo.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);

            delete(id);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridViewInfo.Rows.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            retrieve();
        }
    }
}
