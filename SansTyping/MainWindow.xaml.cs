using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TaeyeonTyping
{
    public partial class MainWindow : Window
    {
        private MediaPlayer[] players;
        private MediaPlayer metroPlayer;
        private int index;

        private List<int> hitList = new List<int>();

        public bool isSoundOff = false;

        public MainWindow()
        {
            InitializeComponent();
            Hook.KeyboardHook.KeyDown += KeyboardHook_KeyDown;
            Hook.KeyboardHook.KeyUp += KeyboardHook_KeyUp;
            Hook.KeyboardHook.HookStart();

            players = new MediaPlayer[5];
            var audioPath = new Uri($"file:///{Path.Combine(Directory.GetCurrentDirectory(), "SansSpeak.wav")}");
            for (var i = 0; i < players.Length; i++)
            {
                players[i] = new MediaPlayer();
                players[i].Open(audioPath);
            }
            metroPlayer = new MediaPlayer();
            metroPlayer.Open(audioPath);
        }

        ~MainWindow()
        {
            Hook.KeyboardHook.HookEnd();
        }

        private bool KeyboardHook_KeyDown(int vkCode)
        {
            if(vkCode == 123)
            {
                return true;
            }
            if (vkCode == 122)
            {
                return true;
            }

            if (isSoundOff)
            {
                return true;
            }

            if (hitList.Contains(vkCode))
            {
                return true;
            }

            hitList.Add(vkCode);
            Play();
            return true;
        }

        int bpm;
        Thread thread;
        bool start = false;

        private void SoundPlay(object args)
        {
            while (true)
            {
                Metro();
                Thread.Sleep((int)args + 100);
                Metro();
                Thread.Sleep((int)args + 100);
                Metro();
                Thread.Sleep((int)args + 100);
                Metro();
                Thread.Sleep((int)args + 100);
            }
        }

        private bool KeyboardHook_KeyUp(int vkCode)
        {
            if (vkCode == 123)
            {
                if(!isSoundOff)
                {
                    OffSound.IsChecked = true;
                }
                else
                {
                    OffSound.IsChecked = false;
                }
                return true;
            }

            if (vkCode == 122)
            {
                if(!start)
                {
                    if(txt_BPM.Text == "")
                    {
                        return true;
                    }
                    start = true;
                    thread = new Thread(new ParameterizedThreadStart(SoundPlay));

                    bpm = Convert.ToInt32(txt_BPM.Text);
                    int Duration = 1000 * 60 / bpm - 100;
                    thread.Start(Duration);

                }
                else
                {
                    start = false;
                    thread.Abort();
                }
            }

            if (isSoundOff)
            {
                return true;
            }
            if (hitList.Contains(vkCode))
            {
                hitList.Remove(vkCode);
            }
            return true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            else
            {
                if(isSoundOff)
                {
                    return;
                }
                Play();
            }
        }

        private void Play()
        {
            players[index].Stop();
            players[index].Play();
            index = (index + 1) % players.Length;
        }

        private void Metro()
        {
            metroPlayer.Dispatcher.Invoke(() =>
            {
                metroPlayer.Stop();
                metroPlayer.Play();
            });
        }

        private void SoundOffSet(object sender, RoutedEventArgs e)
        {
            isSoundOff = true;
        }

        private void SoundOnSet(object sender, RoutedEventArgs e)
        {
            isSoundOff = false;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
