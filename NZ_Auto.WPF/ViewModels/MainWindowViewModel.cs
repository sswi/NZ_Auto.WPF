using NZ_Auto.WPF.DM;
using NZ_Auto.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XE.Commands;

namespace NZ_Auto.WPF.ViewModels
{
  public  class MainWindowViewModel:BindableBase
    {

        public readonly DmSoft _dm = new ();
        private bool _isBind=false;




        //停止鼠标句柄获取跟踪
        private bool _getCursorPosStop=false;
        public bool CursorPosStop
        {
            get => _getCursorPosStop;
            set 
            {
                _getCursorPosStop = value;
                if (value)
                {
                    _getCursorPosStop = true;
                    LoopGetCursorPos(100);
                    WaitHotKey(3);
                }
                else
                {
                    _getCursorPosStop = false;
                }
                OnPropertyChanged();
            }
        }


        //选中的句柄

        private int selectedHandle;
        public int SelectedHandle
        {
            get=> selectedHandle; 
            set { selectedHandle = value; OnPropertyChanged(); }
        }


        //当前句柄
        private int handle;
        public int Handle
        {
            get => handle;
            set { handle = value; OnPropertyChanged(); }
        }

        //鼠标坐标
        private string mousePosition=null!;
        public string MousePosition
        {
            get { return mousePosition; }
            set { mousePosition = value; OnPropertyChanged(); }
        }

        //绑定窗口句柄
        public ICommand BindWindowCommand => new Command(() =>
        {
            int dm_ret = _dm.BindWindow(selectedHandle, "dx2", "windows", "windows", 1);
            if (dm_ret == 1)
            {
                System.Windows.MessageBox.Show("绑定成功!");
                CursorPosStop = false;
                _isBind = true;
            }
            else
            {
                _isBind = false;
            }
        });

        //将窗口移至左上角
        public ICommand MoveWindowToLetAndTopCommand => new Command(() =>
        {
            _dm.MoveWindow(selectedHandle, 0, 0);
        });

        //开始脚本
        public ICommand StartCommand => new Command(() =>
        {


        });













        #region 找图测试

        //调试结果报文
        private string resultMessage=null!;
        public string ResultMessage
        {
            get { return resultMessage; }
            set { resultMessage = value; OnPropertyChanged(); }
        }


        //找图左上角坐标
        public CursorPos LetTopPoint { get; set; } = new CursorPos() { X = 1, Y = 1 };
        //找图右下角坐标
        public CursorPos RightBottomPoint { get; set; } = new CursorPos() { X = 1920, Y = 1080 };

        //找图相似度
        private double similarity = 0.8;
        public double Similarity
        {
            get { return similarity; }
            set { similarity = value; OnPropertyChanged(); }
        }

        //图片路径
        private string picFileName=null!;
        public string PicFileName
        {
            get { return picFileName; }
            set { picFileName = value;OnPropertyChanged();}
        }


        //打开浏览图片
        public ICommand OpenOpenFileDialogCommand => new Command(() =>
        {
            //设置默认的目录为桌面
            var fileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //先读取本地的上次使用目录
            if (File.Exists(Environment.CurrentDirectory + "\\LastFileName"))
            {
                var directory = File.ReadAllText(Environment.CurrentDirectory + "\\LastFileName", Encoding.UTF8);
                if (Directory.Exists(directory))
                {
                    fileName = directory;
                }
            }

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "图片|*.BMP;*.bmp",
                Title = "打开截图",
                InitialDirectory =fileName
            };
            if (openFileDialog.ShowDialog()==true)
            {
                //记录上次所打开的文件夹
                if (fileName != openFileDialog.FileName.Replace(openFileDialog.SafeFileName, ""))
                {
                    File.WriteAllText(Environment.CurrentDirectory + "\\LastFileName", openFileDialog.FileName.Replace(openFileDialog.SafeFileName, ""));
                }
                PicFileName = openFileDialog.FileName;
            }


        });


        //找图
        public ICommand FindPicCommand => new Command(() =>
        {
            if (!_isBind)
            {
                System.Windows.MessageBox.Show("尚未绑定窗口!");
                return;
            }

            if (!File.Exists(picFileName))
            {
                ResultMessage = $"文件不存在:{picFileName}";
                return;
            }

            Task.Run(() =>
            {
                int dm_ret = _dm.FindPic(LetTopPoint.X, LetTopPoint.Y, RightBottomPoint.X, RightBottomPoint.Y, picFileName, "000000", Similarity, 0, out object intX, out object intY);
               ResultMessage = dm_ret == 0? $"找到图片,坐标：{intX}, {intY}" : "未找到";    
            });
        });
        #endregion








        /// <summary>
        /// 循环获取鼠标坐标,和窗口句柄
        /// </summary>
        /// <param name="timeSpan"></param>
        private void LoopGetCursorPos(int timeSpan)
        {
            Task.Run( async() =>
            {
                while (_getCursorPosStop)
                {
                    int dm_ret = _dm.GetCursorPos(out object objX, out object objY);
                    if (dm_ret == 1)
                    {
                        //显示鼠标坐标
                        MousePosition = $"{objX} , {objY}";
                    }
                    else
                    {
                        //坐标获取失败显示-1
                        MousePosition = "-1 , -1";
                    }
                    //获取鼠标所指窗口的句柄
                    Handle = _dm.GetMousePointWindow();

                    //线程休眠，避免CPU超高占用
                    await Task.Delay(timeSpan);
                }
            });
        }


        /// <summary>
        /// 等待快捷键 ALT+A
        /// </summary>
        private  void WaitHotKey(int timeSpan)
        {
            Task.Run(async() =>
            {
                while (_getCursorPosStop)
                {
                    if (_dm.WaitKey(18, 0) + _dm.WaitKey(65, 0) == 2)
                    {
                        SelectedHandle = handle;
                    }
                    //线程休眠，避免CPU超高占用
                    await Task.Delay(timeSpan);
                }
            });
        }



    }
}
