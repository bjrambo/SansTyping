using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SansTyping
{
    public partial class MainWindow : Window
    {
        private MediaPlayer[] players;
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
        }

        ~MainWindow()
        {
            Hook.KeyboardHook.HookEnd();
        }

        private bool KeyboardHook_KeyDown(int vkCode)
        {
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

        private bool KeyboardHook_KeyUp(int vkCode)
        {
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
                Play();
            }
        }

        private void Play()
        {
            players[index].Stop();
            players[index].Play();
            index = (index + 1) % players.Length;
        }

        private void SoundOffSet(object sender, RoutedEventArgs e)
        {
            isSoundOff = true;
        }

        private void SoundOnSet(object sender, RoutedEventArgs e)
        {
            isSoundOff = false;
        }
    }
}
