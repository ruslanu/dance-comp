using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FirstStudioTournamentScheduler
{
    public partial class Form1 : Form
    {

        Scheduler Scheduler = new Scheduler();

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_ReadParticipants_Click(object sender, EventArgs e)
        {
            Scheduler.GenerateMatchSchedule();
        }
    }
}
