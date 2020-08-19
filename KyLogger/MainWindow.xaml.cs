using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KyLogger
{
    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public uint scanCode;
        public KBDLLHOOKSTRUCTFlags flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [Flags]
    public enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_UP = 0x80,
    }



    public partial class MainWindow : Window
    {
        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        private HookProc hCallback = null;
        private IntPtr hHook = IntPtr.Zero;
        private const int WM_KEYDOWN = 0x0100;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (hHook != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hHook);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegisterHook();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void RegisterHook()
        {
            try
            {
                hCallback = new HookProc(HookCallback);
                using(Process process = Process.GetCurrentProcess())
                using (ProcessModule module = process.MainModule)
                {
                    IntPtr hModule = GetModuleHandle(module.ModuleName);
                    hHook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, hCallback, hModule, 0);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code < 0)
                {
                    return CallNextHookEx(hHook, code, wParam, lParam);
                }
                else
                {
                    var _wParam = wParam.ToInt32();
                    switch (_wParam)
                    {
                        case WM_KEYDOWN:
                            {
                                //Debug.WriteLine("WM_KEYDOWN");

                                var kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                                if (kbStruct != null)
                                {
                                    var key = KeyInterop.KeyFromVirtualKey(kbStruct.vkCode);

                                    if (key == Key.Space)
                                    {
                                        kyTextBlock.Text += " ";
                                    }
                                    else if (key == Key.Back)
                                    {
                                        if (kyTextBlock.Text.Length >= 1)
                                            kyTextBlock.Text = kyTextBlock.Text.Remove(kyTextBlock.Text.Length - 1);
                                    }
                                    else if (key == Key.Enter)
                                    {
                                        kyTextBlock.Text += "\n";
                                    }
                                    else if (key == Key.Tab)
                                    {
                                        kyTextBlock.Text += "\t";
                                    }
                                    else if (key == Key.NumPad0 || key == Key.D0)
                                    {
                                        kyTextBlock.Text += "0";
                                    }
                                    else if (key == Key.Oem3)
                                    {
                                        kyTextBlock.Text += "`";
                                    }
                                    else if (key == Key.NumPad1 || key == Key.D1)
                                    {
                                        kyTextBlock.Text += "1";
                                    }
                                    else if (key == Key.NumPad2 || key == Key.D2)
                                    {
                                        kyTextBlock.Text += "2";
                                    }
                                    else if (key == Key.NumPad3 || key == Key.D3)
                                    {
                                        kyTextBlock.Text += "3";
                                    }
                                    else if (key == Key.NumPad4 || key == Key.D4)
                                    {
                                        kyTextBlock.Text += "4";
                                    }
                                    else if (key == Key.NumPad5 || key == Key.D5)
                                    {
                                        kyTextBlock.Text += "5";
                                    }
                                    else if (key == Key.NumPad6 || key == Key.D6)
                                    {
                                        kyTextBlock.Text += "6";
                                    }
                                    else if (key == Key.NumPad7 || key == Key.D7)
                                    {
                                        kyTextBlock.Text += "7";
                                    }
                                    else if (key == Key.NumPad8 || key == Key.D8)
                                    {
                                        kyTextBlock.Text += "8";
                                    }
                                    else if (key == Key.NumPad9 || key == Key.D9)
                                    {
                                        kyTextBlock.Text += "9";
                                    }
                                    else if (key == Key.OemPlus)
                                    {
                                        kyTextBlock.Text += "=";
                                    }
                                    else if (key == Key.OemMinus)
                                    {
                                        kyTextBlock.Text += "-";
                                    }
                                    else if (key == Key.Add)
                                    {
                                        kyTextBlock.Text += "+";
                                    }
                                    else if (key == Key.Subtract)
                                    {
                                        kyTextBlock.Text += "-";
                                    }
                                    else if (key == Key.Multiply)
                                    {
                                        kyTextBlock.Text += "*";
                                    }
                                    else if (key == Key.Divide)
                                    {
                                        kyTextBlock.Text += "/";
                                    }
                                    else if (key == Key.Decimal)
                                    {
                                        kyTextBlock.Text += ".";
                                    }
                                    else if (key == Key.OemOpenBrackets)
                                    {
                                        kyTextBlock.Text += "[";
                                    }
                                    else if (key == Key.Oem6)
                                    {
                                        kyTextBlock.Text += "]";
                                    }
                                    else if (key == Key.Oem5)
                                    {
                                        kyTextBlock.Text += "\\";
                                    }
                                    else if (key == Key.Oem1)
                                    {
                                        kyTextBlock.Text += ";";
                                    }
                                    else if (key == Key.OemQuotes)
                                    {
                                        kyTextBlock.Text += "\'";
                                    }
                                    else if (key == Key.OemComma)
                                    {
                                        kyTextBlock.Text += ",";
                                    }
                                    else if (key == Key.OemPeriod)
                                    {
                                        kyTextBlock.Text += ".";
                                    }
                                    else if (key == Key.OemQuestion)
                                    {
                                        kyTextBlock.Text += "/";
                                    }
                                    else
                                    {
                                        kyTextBlock.Text += $"{key}";
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    return CallNextHookEx(hHook, code, wParam, lParam);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
