using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.WinForms;
using System.Runtime.InteropServices;

namespace KR
{
    public partial class GeneralMenu : Form
    {

        DataBase dataBase = new DataBase();


        public GeneralMenu()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

        }

      

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Client client = new Client();

            client.Show();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Worked personal = new Worked();

            personal.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Project project = new Project();
            project.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Service service = new Service();
            service.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            TypeFormat typeFormat = new TypeFormat();
            typeFormat.Show();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            TypeProject typeProject = new TypeProject();    
            typeProject.Show();
        }

        private void рекламныеКаналыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AD aD = new AD();
            aD.Show();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Payment payment = new Payment();
            payment.Show();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            ProkectWorked prokectWorked = new ProkectWorked();
            prokectWorked.Show();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            SearchDate searchDate = new SearchDate();
            searchDate.Show();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            SearchStatus searchStatus = new SearchStatus();
            searchStatus.Show();
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            SearchPayment payment = new SearchPayment();
            payment.Show();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            SearchClient clientsear = new SearchClient();   
            clientsear.Show();
        }

        private void отчётToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportForm reportForm = new ReportForm();
            reportForm.Show();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            applicationsClients applicationsClients = new applicationsClients();
            applicationsClients.Show();
            
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            querySelection Query = new querySelection();
            Query.Show();
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
