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
using System.Data.SqlClient;
using System.Data.Odbc;
namespace Products
{
    public partial class Users : frmInheritance
    {
        bool boolUserExists = false;
        int intUserID = 0;

        string strAccessConnectionString = "Driver= {Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb;Uid=Admin;Pwd=;";

        public Users()
        {
            InitializeComponent();
        }

        private void Users_Load(object sender, EventArgs e)
        {

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
             if (btnAdd.Text == "ADD")
            {
                if(txtUsername.Text == "")
                {
                    MessageBox.Show("User Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPassword.Text == "")
                {
                    MessageBox.Show("Password field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
               
                else
                {
                
                checkIfUserExists();
                if(boolUserExists == false)
                {
                    createUser();
                    controlsLoad();
                    ClearTextBoxes();
                    loadUser();
                }
                else if (boolUserExists == true)
                {
                    MessageBox.Show("User already exists", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
           }
        }
       
            else if (btnAdd.Text == "Add")
            {
                controlsAdd();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            editUser();
            controlsEdit();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteUser();
            controlsLoad();
            ClearTextBoxes();
            loadUser();
        }

        private void controlsLoad()
        {
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            

            cboUsers.Enabled = true;

            btnAdd.Enabled = true;
            btnDelete.Enabled = false;
            btnEdit.Enabled = true;
            btnReturn.Enabled = true;
            btnRemove.Enabled = false;

            btnAdd.Text = "ADD";
        }

        private void controlsAdd()
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
        

            cboUsers.Enabled = false;

            btnAdd.Enabled = true;
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
            btnReturn.Enabled = false;
            btnRemove.Enabled = false;


            btnAdd.Text = "ADD";

        }

        private void controlsEdit()
        {
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
          

            cboUsers.Enabled = false;

            btnAdd.Enabled = false;
            btnDelete.Enabled = true;
            btnEdit.Enabled = false;
            btnReturn.Enabled = false;
            btnRemove.Enabled = true;


           // btnCreate.Text = "Save";

        }

        private void ClearTextBoxes()
        {
             txtUsername.Text = "";
            txtPassword.Text = "";
           
        }

        private void loadUser()
        {
            cboUsers.DataSource = null;
            cboUsers.Items.Clear();


            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcConnection.ConnectionString = strAccessConnectionString;

            string query = "SELECT UserName FROM Users";
            OdbcCommand cmd = new OdbcCommand(query, OdbcConnection);

            OdbcConnection.Open();

            OdbcDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection ProductCollection = new AutoCompleteStringCollection();
            while(dr.Read())
            {
                ProductCollection.Add(dr.GetString(0));

            }
            cboUsers.DataSource = ProductCollection;
            OdbcConnection.Close();
        }
        private void createUser()
        {
            string query = "SELECT * FROM Users WHERE ID=0";
            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;
            da.Fill(ds,"Users");
            dt = ds.Tables["Users"];

            try
            {
                dr = dt.NewRow();
                dr["UserName"] = txtUsername.Text;
                dr["Password"] = txtPassword.Text;
               

                dt.Rows.Add(dr);
                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);

                da.Update(ds, "Users");
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }

        }

        private void checkIfUserExists()
        {
            string query = "SELECT * FROM Users WHERE UserName='" + txtUsername.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();   
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            //OdbcDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                boolUserExists = true;
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        private void editUser()
        {
            string query = "SELECT * FROM Users WHERE UserName='" + cboUsers.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);
            OdbcCommand cmd;
            OdbcDataReader dr;

            //OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                intUserID = dr.GetInt32(0);
                txtUsername.Text = dr.GetString(1);
                txtPassword.Text = dr.GetString(2);
                
            }
            //dr.Close();
            //OdbcConnection.Close();
            //dr.Dispose();
            //OdbcConnection.Dispose();

            DataSet ds = new DataSet("Users");
            da.FillSchema(ds, SchemaType.Source, "Users");
            da.Fill(ds, "Users");
            DataTable dt;
            dt = ds.Tables["Users"];
            DataRow dr1;
            dr1 = dt.NewRow();

            try
            {
                dr1 = dt.Rows.Find(intUserID);
                dr1.BeginEdit();

                dr1["UserName"] = txtUsername.Text;
                dr1["Password"] = txtPassword.Text;
                

                dr1.EndEdit();

                OdbcCommandBuilder command = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");


         


            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }

        }

       

        private void deleteUser()
        {
            string query = "DELETE FROM Users WHERE ID=" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();

            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            //OdbcDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        

        }
    
}
