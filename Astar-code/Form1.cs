using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astar
{   
    

    public partial class Form1 : Form
    {

        Button[,] buttons;

        Button[,] buttons_end;

        Button[,] buttons_demo;

        int cnt;

        double interval;



        List<point> l;

        class point//表示八数码问题的一个状态
        {
            public int f;//f=g+h
            public int x, y;//表示空格的位置
            public int g;//表示已经付出的代价，计算已经付出的代价的时候需要先指定父节点
            public int h;//表示预测还需要付出的代价
            public int[,] state;//表示当前的状态
            public point parent;//表示当前节点的父节点
            public point(int[,] cin)
            {
                int i, j;
                state = new int[3, 3];
                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        this.state[i, j] = cin[i, j];

                this.getblanck();//初始化节点，给出节点的状态，然后从状态中得到空格的位置
            }
            public point(point tmp)//对传入的point进行一次复制
            {
                int i, j;
                this.state = new int[3, 3];
                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        this.state[i, j] = tmp.state[i, j];

                this.getblanck();
            }
            public point()
            {
                
            }
            public void setG()//g表示已经付出的代价初始节点的代价为0其余节点的代价为父节点的代价加1
            {
                this.g = (this.parent == null ? 0 : this.parent.g + 1);
            }//先设置父节点再求g
            public void setH(point end)//预测和终点的距离，采用和终点不相同的格子的数量来表示
            {
                int i, j;
                int cnt = 0;

                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        if (this.state[i, j] != end.state[i, j]) cnt++;

                this.h = cnt;
            }
            public void setF()//g+h即为f
            {   
                
                f = g + h;
            }
            public void getblanck()//获得空格的位置
            {
                int i, j;

                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        if (this.state[i, j] == 0)
                        {
                            this.x = i;
                            this.y = j;
                        }
            }
        }
        class Astar_test
        {
            public List<point> open;
            public List<point> close;
            public point start, end;

            public Astar_test(point start, point end)
            {
                open = new List<point>();
                close = new List<point>();
                this.start = start;
                this.end = end;
            }

            public point exe()//执行A*算法
            {
                point exam = null;//待考察的节点
                point newpoint;//新生成的节点
                point tmp;//表示在open表或者close表中的节点

                int i;

                open.Add(start);//将初始节点加入到open表中

                while (open.Count > 0)
                {
                    //在open表中找一个f最小的节点
                    exam = getminf(open);

                    if (issame(exam, end)) break;//表示已经找到了目标节点

                    else
                    {
                        for (i = 0; i < 4; i++)
                        {
                            if (canmoveto(exam, i))//表明没有越界，还要观察是否是父节点
                            {
                                newpoint = moveto(exam, i);

                                if (issame(newpoint, exam.parent))
                                {
                                    continue;//如果是父节点的话放弃操作

                                }
                                else//不是父节点也可以到达，接下来检查是否在open和close表中
                                {
                                    newpoint.parent = exam;
                                    newpoint.setH(end);
                                    newpoint.setG();
                                    newpoint.setF();
                                    
                                    if (isinlist(newpoint, open) != null)
                                    { //在open表中
                                        tmp = isinlist(newpoint, open);//得到在open表里的这个节点

                                        if (tmp.f > newpoint.f)//找到了一条更优的路径
                                        {
                                            //更新在open表中的这个节点
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.setF();
                                            continue;

                                        }

                                    }
                                    else if (isinlist(newpoint, close) != null)//不在open表中但是在close表中
                                    {
                                        tmp = isinlist(newpoint, close);//得到在close表中的这个节点

                                        if (tmp.f > newpoint.f)//找到了到这个节点的一条更优路径，把这个节点移出close表加入open表
                                        {
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.setH(end);
                                            tmp.setF();
                                            close.Remove(tmp);
                                            open.Add(tmp);
                                            continue;
                                        }
                                    }
                                    else//既不在open表里也不在close表里
                                    {
                                        open.Add(newpoint);
                                    }
                                }
                            }
                        }
                    }

                    //考察完当前节点了之后把其加入到close表中
                    open.Remove(exam);
                    close.Add(exam);
                }

                return exam;
            }

            public point getminf(List<point> list)//在列表中寻找一个F值最小的节点
            {
                int min;
                point tmp;

                tmp = list[0];

                min = tmp.f;

                foreach (point pit in list)
                {
                    if (pit.f < min)
                    {
                        min = pit.f;

                        tmp = pit;
                    }
                }

                return tmp;
            }
            public bool canmoveto(point cin_p, int direction)//根据输入的direction来判断当前节点是否可以移动到目标位置
            {
                switch (direction)//0向左1向右2向上3向下
                {
                    case 0:
                        if (cin_p.x == 0) return false;
                        break;
                    case 1:
                        if (cin_p.x == 2) return false;
                        break;
                    case 2:
                        if (cin_p.y == 0) return false;
                        break;
                    case 3:
                        if (cin_p.y == 2) return false;
                        break;

                }

                return true;


            }
            public point moveto(point cin_p, int direciton)
            {
                point tmp = new point(cin_p);
                int buffer;

                switch (direciton)
                {
                    case 0:
                        buffer = tmp.state[tmp.x - 1, tmp.y];

                        tmp.state[tmp.x - 1, tmp.y] = tmp.state[tmp.x, tmp.y];

                        tmp.state[tmp.x, tmp.y] = buffer;

                        break;

                    case 1:

                        buffer = tmp.state[tmp.x + 1, tmp.y];

                        tmp.state[tmp.x + 1, tmp.y] = tmp.state[tmp.x, tmp.y];

                        tmp.state[tmp.x, tmp.y] = buffer;

                        break;

                    case 2:

                        buffer = tmp.state[tmp.x, tmp.y - 1];

                        tmp.state[tmp.x, tmp.y - 1] = tmp.state[tmp.x, tmp.y];

                        tmp.state[tmp.x, tmp.y] = buffer;

                        break;

                    case 3:

                        buffer = tmp.state[tmp.x, tmp.y + 1];

                        tmp.state[tmp.x, tmp.y + 1] = tmp.state[tmp.x, tmp.y];

                        tmp.state[tmp.x, tmp.y] = buffer;

                        break;

                }

                tmp.getblanck();

                return tmp;
            }
            public bool issame(point p1, point p2)
            {
                int i, j;

                if (p1 == null || p2 == null)
                    return false;

                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        if (p1.state[i, j] != p2.state[i, j]) return false;

                return true;
            }
            public point isinlist(point p1, List<point> list)//如果在的话返回这个节点，如果没有则返回一个null
            {
                int i, j;
                foreach (var tmp in list)
                {
                    if (issame(p1, tmp)) return tmp;

                }

                return null;
            }

            
           

            public static bool cangettoTarget(string s1 ,string s2)//判断终点是否可达，计算逆序数
            {
             
                int value1=0, value2=0;//分别表示起点和终点的逆序数
                int i,j;

                for (i = 0; i < s1.Length; i++)
                {
                    char tmp = s1[i];//取第i个字符
                    int cnt = 0;

                    if (tmp == '0')//省略0
                    {
                        continue;
                    }

                    for (j = i - 1; j >= 0; j--)
                    {
                        if (s1[j] - '0' > s1[i]-'0') cnt++;
                    }

                    value1 += cnt;
                }

                for (i = 0; i < s2.Length; i++)
                {
                    char tmp = s2[i];//取第i个字符
                    int cnt = 0;

                    if (tmp == '0')
                    {
                        continue;
                    }

                    for (j = i - 1; j >= 0; j--)
                    {
                        if (s2[j] - '0' > s2[i] - '0') cnt++;
                    }

                    value2 += cnt;
                }

                if (value1 % 2 == value2 % 2)//如果说同奇同偶
                {
                    return true;

                }
                else
                    return false;

            }

            public static void converttobuttons(point p1, Button[,] buttons_in)//把p1的状态显示到按钮控件上去
            {
                int i, j;

                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                    {

                        if (p1.state[i, j] == 0)
                        {
                            buttons_in[i, j].Visible = false;
                            buttons_in[i, j].Text = 0 + "";
                        }
                        else
                        {
                            buttons_in[i, j].Visible = true;
                            buttons_in[i, j].Text =p1.state[i, j]+"";
                        }
                        
                    }
                
            }
        }

        class Global_optimun : Astar_test
        {
            public Global_optimun(point start, point end) : base(start,end)
            {
                
            }

            public point exe()
            {
                point exam = null;//待考察的节点
                point newpoint;//新生成的节点
                point tmp;//表示在open表或者close表中的节点

                int i;

                open.Add(start);//将初始节点加入到open表中

                while (open.Count > 0)
                {
                    //在open表中找一个f最小的节点
                    exam = getminf(open);

                    if (issame(exam, end)) break;//表示已经找到了目标节点

                    else
                    {
                        for (i = 0; i < 4; i++)
                        {
                            if (canmoveto(exam, i))//表明没有越界，还要观察是否是父节点
                            {
                                newpoint = moveto(exam, i);

                                if (issame(newpoint, exam.parent))
                                {
                                    continue;//如果是父节点的话放弃操作

                                }
                                else//不是父节点也可以到达，接下来检查是否在open和close表中
                                {
                                    newpoint.parent = exam;
                                    newpoint.setH(end);
                                    newpoint.setG();
                                    newpoint.f = newpoint.h;//全局择优
                                    

                                    if (isinlist(newpoint, open) != null)
                                    { //在open表中
                                        tmp = isinlist(newpoint, open);//得到在open表里的这个节点

                                        if (tmp.f > newpoint.f)//找到了一条更优的路径
                                        {
                                            //更新在open表中的这个节点
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.f = tmp.h;//全局择优
                                            continue;

                                        }

                                    }
                                    else if (isinlist(newpoint, close) != null)//不在open表中但是在close表中
                                    {
                                        tmp = isinlist(newpoint, close);//得到在close表中的这个节点

                                        if (tmp.f > newpoint.f)//找到了到这个节点的一条更优路径，把这个节点移出close表加入open表
                                        {
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.setH(end);
                                            tmp.f = tmp.h;//全局择优
                                            close.Remove(tmp);
                                            open.Add(tmp);
                                            continue;
                                        }
                                    }
                                    else//既不在open表里也不在close表里
                                    {
                                        open.Add(newpoint);
                                    }
                                }
                            }
                        }
                    }

                    //考察完当前节点了之后把其加入到close表中
                    open.Remove(exam);
                    close.Add(exam);
                }

                return exam;
            }


        }

        class Width_first : Astar_test
        {
            public Width_first(point start, point end) : base(start, end)
            {
                
            }

            public point exe()
            {
                point exam = null;//待考察的节点
                point newpoint;//新生成的节点
                point tmp;//表示在open表或者close表中的节点

                int i;

                open.Add(start);//将初始节点加入到open表中

                while (open.Count > 0)
                {
                    //在open表中找一个f最小的节点
                    exam = getminf(open);

                    if (issame(exam, end)) break;//表示已经找到了目标节点

                    else
                    {
                        for (i = 0; i < 4; i++)
                        {
                            if (canmoveto(exam, i))//表明没有越界，还要观察是否是父节点
                            {
                                newpoint = moveto(exam, i);

                                if (issame(newpoint, exam.parent))
                                {
                                    continue;//如果是父节点的话放弃操作

                                }
                                else//不是父节点也可以到达，接下来检查是否在open和close表中
                                {
                                    newpoint.parent = exam;
                                    newpoint.setH(end);
                                    newpoint.setG();
                                    newpoint.f = newpoint.g;//广度优先


                                    if (isinlist(newpoint, open) != null)
                                    { //在open表中
                                        tmp = isinlist(newpoint, open);//得到在open表里的这个节点

                                        if (tmp.f > newpoint.f)//找到了一条更优的路径
                                        {
                                            //更新在open表中的这个节点
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.f = tmp.g;//广度优先
                                            continue;

                                        }

                                    }
                                    else if (isinlist(newpoint, close) != null)//不在open表中但是在close表中
                                    {
                                        tmp = isinlist(newpoint, close);//得到在close表中的这个节点

                                        if (tmp.f > newpoint.f)//找到了到这个节点的一条更优路径，把这个节点移出close表加入open表
                                        {
                                            tmp.parent = exam;
                                            tmp.setG();
                                            tmp.setH(end);
                                            tmp.f = tmp.g;//广度优先
                                            close.Remove(tmp);
                                            open.Add(tmp);
                                            continue;
                                        }
                                    }
                                    else//既不在open表里也不在close表里
                                    {
                                        open.Add(newpoint);
                                    }
                                }
                            }
                        }
                    }

                    //考察完当前节点了之后把其加入到close表中
                    open.Remove(exam);
                    close.Add(exam);
                }

                return exam;
            }

        }

        public Form1()
        {
            InitializeComponent();
        }

         List<point> out_put(point p1)//根据最终得到的目标节点来反向求得路径,并且在buttons控件中显示出来
        {
            point tmp = p1;
            
            int i;
            

            List<point> l1 = new List<point>();
            l1.Add(tmp);

            while (tmp.parent != null)
            {
                tmp = tmp.parent;
                l1.Add(tmp);

            }

            cnt = l1.Count;

            interval = 100.0 / cnt;

           

            return l1;





        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int i, j ;

            this.buttons = new Button[3, 3];

            this.buttons_end = new Button[3, 3];

            this.buttons_demo = new Button[3, 3];

            this.KeyPreview = true;

            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                {
                    buttons[i, j] = new Button();

                    // buttons[i, j].SetBounds(37 + i * 57, 74 + j * 94, 100, 100);

                    buttons[i, j].Top = 30 + j * 45;

                    buttons[i, j].Left = 21 + i * 45;

                    buttons[i, j].Width = 41;

                    buttons[i, j].Height = 41;

                    buttons[i, j].Visible = true;

                    this.Controls.Add(buttons[i,j]);
                    
                }

            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                {
                    buttons_end[i, j] = new Button();

                    buttons_end[i, j].Top = 180 + j * 45;

                    buttons_end[i, j].Left = 21 + i * 45;

                    buttons_end[i, j].Width = 41;

                    buttons_end[i, j].Height = 41;

                    buttons_end[i, j].Visible = true;

                    this.Controls.Add(buttons_end[i, j]);

                }

            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                {
                    buttons_demo[i, j] = new Button();

                    buttons_demo[i, j].Top = 180 + j * 45;

                    buttons_demo[i, j].Left = 170 + i * 45;

                    buttons_demo[i, j].Width = 41;

                    buttons_demo[i, j].Height = 41;

                    buttons_demo[i, j].Visible = true;

                    this.Controls.Add(buttons_demo[i, j]);

                }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            point start, end;
            point target;
            
            int i ,tmp , x , y;
            int[,] state;
            string text1 = this.textBox1.Text;//初始状态
            string text2 = this.textBox2.Text;//目标状态

            state = new int[3, 3];

            for (i = 0; i < text2.Length; i++)
            {
                x = i % 3;
                y = i / 3;

                state[x, y] = text2[i] - '0';
            }

            end = new point(state);

            for (i = 0; i < text1.Length; i++)
            {
                x = i % 3;
                y = i / 3;

                state[x, y] = text1[i] - '0';
            }

            start = new point(state);

            start.setG();
            start.setH(end);
            //start.setF();

            if (this.checkBox1.Checked)//执行A*算法
            {
                start.setF();

                Astar_test astar_Test = new Astar_test(start, end);

                if (Astar_test.cangettoTarget(text1, text2))//如果说可以到达
                {
                    Astar_test.converttobuttons(start, buttons);

                    Astar_test.converttobuttons(end, buttons_end);

                    Astar_test.converttobuttons(start, buttons_demo);

                    target = astar_Test.exe();//得到最终节点，反向求得路径

                    this.progressBar1.Value = 0;

                    this.l = out_put(target);

                    this.textBox4.Text = 0 + "";
                }

                else
                    MessageBox.Show("终点无法到达！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (this.checkBox2.Checked)//全局择优算法
            {
                start.f = start.h;

                Global_optimun global_Optimun = new Global_optimun(start, end);

                if (Global_optimun.cangettoTarget(text1, text2))//如果说可以到达
                {
                    Global_optimun.converttobuttons(start, buttons);

                    Global_optimun.converttobuttons(end, buttons_end);

                    Global_optimun.converttobuttons(start, buttons_demo);

                    target = global_Optimun.exe();//得到最终节点，反向求得路径

                    this.progressBar1.Value = 0;

                    this.l = out_put(target);

                    this.textBox4.Text = 0 + "";
                }

                else
                    MessageBox.Show("终点无法到达！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (this.checkBox3.Checked)//广度优先
            {
                start.f = start.g;

                Width_first width_first = new Width_first(start, end);

                if (Width_first.cangettoTarget(text1, text2))//如果说可以到达
                {
                    Width_first.converttobuttons(start, buttons);

                    Width_first.converttobuttons(end, buttons_end);

                    Width_first.converttobuttons(start, buttons_demo);

                    target = width_first.exe();//得到最终节点，反向求得路径

                    this.progressBar1.Value = 0;

                    this.l = out_put(target);

                    this.textBox4.Text = 0 + "";
                }

                else
                    MessageBox.Show("终点无法到达！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (this.checkBox4.Checked)//玩游戏
            {
                

                Astar_test.converttobuttons(start, buttons);

                Astar_test.converttobuttons(end, buttons_end);

                Astar_test.converttobuttons(start, buttons_demo);

                
            }
            

            
        }

        public static bool gettoend(Button[,] cin, Button[,] end)
        {
            int i, j;

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    if (!(cin[i, j].Text == end[i, j].Text)) return false;
                }
            }

            return true;
        }
        

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            int cin = Convert.ToInt32(this.textBox4.Text);//得到当前步数

            this.progressBar1.Value = (int)(cin * interval);

            if (cin == cnt-1)
            {
                if (this.progressBar1.Value != 100)
                {
                    this.progressBar1.Value = 100;
                }

                MessageBox.Show("成功到达终点演示结束！", "恭喜", MessageBoxButtons.OK);
            }
            else
            {
                Astar_test.converttobuttons(l[cnt - cin - 2], this.buttons_demo);

                cin++;

                this.textBox4.Text = cin + "";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
          
        }

       

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {       
              
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int i, j;
            int tmp;

           

            if (this.checkBox4.Checked)
            {
                 if (gettoend(buttons_demo, buttons_end)&&buttons_demo[1,1].Text!="")
            {
                MessageBox.Show("您已经成功到达终点!", "恭喜!", MessageBoxButtons.OK);
                return;
            }


                if (e.KeyData==Keys.W)//用户输入上键
                {
                    

                    //判断空格所在的位置
                    for (i = 0; i < 3; i++)
                        for (j = 0; j < 3; j++)
                        {
                            if (this.buttons_demo[i, j].Text == 0+"")//找到空格
                            {
                                if (j == 0) return;

                                else//上移
                                {
                                    tmp = int.Parse(this.buttons_demo[i, j - 1].Text);
                                    buttons_demo[i, j - 1].Visible = false;
                                    buttons_demo[i, j - 1].Text = "0";
                                    buttons_demo[i, j].Text = tmp+"";
                                    buttons_demo[i, j].Visible = true;
                                    return;
                                }
                            }
                        }
                }
                else if (e.KeyData == Keys.S)
                {
                    for (i = 0; i < 3; i++)
                        for (j = 0; j < 3; j++)
                        {
                            if (this.buttons_demo[i, j].Text == "0")//找到空格
                            {
                                if (j == 2) return;

                                else//下移
                                {
                                    tmp = int.Parse(this.buttons_demo[i, j + 1].Text);
                                    buttons_demo[i, j + 1].Visible = false;
                                    buttons_demo[i, j + 1].Text = "0";
                                    buttons_demo[i, j].Text = tmp + "";
                                    buttons_demo[i, j].Visible = true;
                                    return;
                                }
                            }
                        }
                }
                else if (e.KeyData == Keys.A)
                {
                    

                    for (i = 0; i < 3; i++)
                        for (j = 0; j < 3; j++)
                        {
                            if (this.buttons_demo[i, j].Text == "0")//找到空格
                            {
                                if (i == 0) return;

                                else//左移
                                {
                                    tmp = int.Parse(this.buttons_demo[i - 1, j].Text);
                                    buttons_demo[i - 1, j].Visible = false;
                                    buttons_demo[i - 1, j].Text = "0";
                                    buttons_demo[i, j].Text = tmp + "";
                                    buttons_demo[i, j].Visible = true;
                                    return;
                                }
                            }
                        }
                }
                else if (e.KeyData == Keys.D)
                {
                    for (i = 0; i < 3; i++)
                        for (j = 0; j < 3; j++)
                        {
                            if (this.buttons_demo[i, j].Text == "0")//找到空格
                            {
                                if (i == 2) return;

                                else//右移
                                {
                                    tmp = int.Parse(this.buttons_demo[i + 1, j].Text);
                                    buttons_demo[i + 1, j].Visible = false;
                                    buttons_demo[i + 1, j].Text = "0";
                                    buttons_demo[i, j].Text = tmp + "";
                                    buttons_demo[i, j].Visible = true;
                                    return;
                                }
                            }
                        }
                }

                if (gettoend(buttons_demo, buttons_end) && buttons_demo[1, 1].Text != "")
                {
                    MessageBox.Show("您已经成功到达终点!", "恭喜!", MessageBoxButtons.OK);
                    return;
                }
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
