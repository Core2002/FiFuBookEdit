﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FiFu书编辑器
{
    public partial class Form1 : Form
    {
        static FiFuBook 正在浏览的书;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            正在浏览的书.change(textBox1.Text);
        }

        /// <summary>
        /// 打开书文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择书文件";
            dialog.Filter = "书文件(*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string file = dialog.FileName;
                正在浏览的书 = new FiFuBook(file);
                if (正在浏览的书.不合法)
                {
                    MessageBox.Show("选择的书文件不合法");
                    return;
                }

                textBox1.Text = 正在浏览的书.getPage(0);
                label1.Text = 正在浏览的书.getPageNumText();

                this.Text = "FiFu 书编辑器 - " + 正在浏览的书.path;

                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                textBox1.Enabled = true;
            }
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = 正在浏览的书.previousPage();
            label1.Text = 正在浏览的书.getPageNumText();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = 正在浏览的书.nextPage();
            label1.Text = 正在浏览的书.getPageNumText();
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();  //实例化保存文件对话框对象
            saveFile.Title = "请选择保存文件路径";
            saveFile.Filter = "书文件(*.txt)|*.txt";
            saveFile.OverwritePrompt = true;  //是否覆盖当前文件
            saveFile.RestoreDirectory = true;  //还原目录
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFile.FileName, 正在浏览的书.toJsonText());

            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            File.WriteAllText(正在浏览的书.path, 正在浏览的书.toJsonText());
            // MessageBox.Show("另存文档成功");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 添加页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            正在浏览的书.addPage();
            label1.Text = 正在浏览的书.getPageNumText();
        }

        /// <summary>
        /// 删除页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            正在浏览的书.delPage();
            label1.Text = 正在浏览的书.getPageNumText();
            textBox1.Text = 正在浏览的书.getPage();
        }

        /// <summary>
        /// 新建书文件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (正在浏览的书 != null)
            {
                DialogResult dr = MessageBox.Show("确定要保存当前文件嘛？", "当前已打开了一个书文件", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    File.WriteAllText(正在浏览的书.path, 正在浏览的书.toJsonText());
                }
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "请选择新建书文件的路径";
            saveFile.Filter = "书文件(*.txt)|*.txt";
            saveFile.OverwritePrompt = true;
            saveFile.RestoreDirectory = true;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFile.FileName, "[\"\"]");
                正在浏览的书 = new FiFuBook(saveFile.FileName);
                if (正在浏览的书.不合法)
                {
                    MessageBox.Show("选择的书文件不合法");
                    return;
                }

                textBox1.Text = 正在浏览的书.getPage(0);
                label1.Text = 正在浏览的书.getPageNumText();

                this.Text = "FiFu 书编辑器 - " + 正在浏览的书.path;

                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                textBox1.Enabled = true;
            }
        }
    }


    public class FiFuBook
    {
        public bool 不合法 = false;
        List<string> book;
        int[] p = { 0, 0 };
        public string path;
        public FiFuBook(string bookFile)
        {
            var f = File.OpenText(bookFile);
            String bookJson = f.ReadToEnd();
            book = JsonConvert.DeserializeObject<List<string>>(bookJson);
            if (book == null)
            {
                不合法 = true;
                return;
            }
            path = bookFile;
            p[0] = 0;
            p[1] = getMaxPageNum();
            f.Close();
        }

        public string toJsonText()
        {
            return JsonConvert.SerializeObject(book);
        }

        public string getPage(int page)
        {
            return book[page];
        }

        public string getPage()
        {
            return book[p[0]];
        }

        public int getMaxPageNum()
        {
            return book.Count();
        }

        public string nextPage()
        {
            ++p[0];
            if (p[0] < p[1] && p[0] >= 0)
            {
                return getPage(p[0]);
            }
            else
            {
                p[0] = 0;
                return getPage(p[0]);
            }
        }

        public string previousPage()
        {
            --p[0];
            if (p[0] < p[1] && p[0] >= 0)
            {
                return getPage(p[0]);
            }
            else
            {
                p[0] = p[1] - 1;
                return getPage(p[0]);
            }
        }

        public void change(String text)
        {
            book[p[0]] = text;
        }

        public void addPage()
        {
            book.Add("");
            p[1] = getMaxPageNum();
        }

        public void delPage()
        {
            if (p[0] == 0)
            {
                book[0] = "";
                return;
            }
            book.Remove(getPage(p[0]));
            p[1] = getMaxPageNum();
            previousPage();
        }

        public string getPageNumText()
        {
            return "第 " + (p[0] + 1) + " 页/共 " + p[1] + " 页";
        }
    }
}
