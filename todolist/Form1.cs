﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace todolist
{
    public partial class Form1 : Form
    {
        private Boolean showPanelTask = false;

        public Form1()
        {
            InitializeComponent();
            tooglePanels();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            dgvToDo.Columns["StartDate"].DefaultCellStyle.Format = "MM/dd/yyyy hh:mm tt";
            loadFile(dgvToDo, "todolist.txt");

            timer2.Start();
        }

        private void btnTask_Click(object sender, EventArgs e)
        {
            showPanelTask = !showPanelTask;
            lblStatus.Text = "TO DO LIST";
            tooglePanels();
        }

        private void tooglePanels()
        {
            if (showPanelTask)
            {
                panelTask.Height = 128;
            }
            else
            {
                panelTask.Height = 0;
            }
        }

        //show all tasks
        private void btnAll_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "TO DO LIST | ALL TASKS";
            foreach (DataGridViewRow row in dgvToDo.Rows)
            {
                row.Visible = true;
            }
        }

        //show completed tasks
        private void btnCompleted_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "TO DO LIST | COMPLETED";
            foreach (DataGridViewRow row in dgvToDo.Rows)
            {
                bool status = Convert.ToBoolean(row.Cells[0].Value);
                row.Visible = status;
            }
        }

        //show incompleted tasks
        private void btnIncomplete_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "TO DO LIST | INCOMPLETE";
            foreach (DataGridViewRow row in dgvToDo.Rows)
            {
                bool status = Convert.ToBoolean(row.Cells[0].Value);
                row.Visible = !status;
            }
        }

        //exit
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        //show date
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDate.Text = DateTime.Now.ToLongDateString();
        }

        //add task
        public void addTaskToDGV(bool status, string task, DateTime startdate, int remind, string tag, string note)
        {
            dgvToDo.Rows.Add(status, task, startdate, remind, tag, note);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAddTask f = new FrmAddTask(this);
            f.Show();
        }

        //delete selected row
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvToDo.Rows.Count > 0)
            {
                dgvToDo.Rows.RemoveAt(dgvToDo.SelectedRows[0].Index);
            }
            else if(dgvToDo.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to delete.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //save file
        public void saveFile(DataGridView dataGridView, string filePath)
        {
            using (StreamWriter writer = new StreamWriter("todolist.txt", false))
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataGridViewRow row in dgvToDo.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value is bool boolValue)
                            sb.Append(boolValue.ToString() + ",");
                        else if (cell.Value is DateTime dateTimeValue)
                            sb.Append(dateTimeValue.ToString("MM/dd/yyyy hh:mm tt") + ",");
                        else if (cell.Value is int intValue)
                            sb.Append(intValue.ToString() + ",");
                        else
                            sb.Append(cell.Value?.ToString() + ",");
                    }
                    sb.AppendLine();
                }
                writer.Write(sb.ToString());
            }
        }

        private void loadFile(DataGridView dataGridView, string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                dataGridView.Rows.Clear();

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');
                    object[] rowData = new object[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (bool.TryParse(values[i], out bool boolValue))
                            rowData[i] = boolValue;
                        else if (DateTime.TryParse(values[i], out DateTime dateTimeValue))
                            rowData[i] = dateTimeValue;
                        else if (int.TryParse(values[i], out int intValue))
                            rowData[i] = intValue;
                        else
                            rowData[i] = values[i];
                    }
                    dataGridView.Rows.Add(values);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveFile(dgvToDo, "todolist.txt");
        }

        private void Notification()
        {
            DateTime current = DateTime.Now;
            foreach (DataGridViewRow row in dgvToDo.Rows)
            {
                if (row.Cells["StartDate"].Value != null && row.Cells["Remind"].Value !=null)
                {
                    string deadlinestr = row.Cells["StartDate"].Value.ToString();
                    DateTime deadline;

                    string minuteRemainstr = row.Cells["Remind"].Value.ToString();
                    int minuteRemain;

                    bool isNotified = Convert.ToBoolean(row.Cells["Notified"].Value);
                    
                    if (int.TryParse(minuteRemainstr, out minuteRemain))
                    {
                        if (DateTime.TryParse(deadlinestr, out deadline))
                        {
                            TimeSpan timeRemaining = deadline - current;
                            int m = (int)timeRemaining.TotalMinutes;

                            if (m <= minuteRemain && m > 0 && !isNotified)
                            {
                                MessageBox.Show($"Start doing task '{row.Cells["Task"].Value}' in {m} minutes!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                row.Cells["Notified"].Value = "true";
                            }
                        }
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Notification();
        }


    }
}
