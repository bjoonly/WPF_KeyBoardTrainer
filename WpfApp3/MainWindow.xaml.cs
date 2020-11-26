using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Threading;

namespace WpfApp3
{

    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
       private int countSec=0;
        private int countSymbols=0;
       private string text;
        private List<string> allSentences;
        Random random = new Random();
        private List<string> currentSentences;
        private bool isCaseSensitivity = true;
        private int countSentences;
        bool isCorrect = true;
        private int currentSymbol;
        private int currentSentence;
        int rand=0;
        
        int countMistakes = 0;
        int symbolsInMinute;
        double avgSymbInMinute;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            using (StreamReader sr = new StreamReader(@"text.txt"))
            {
                text = sr.ReadToEnd();
            }
            allSentences = new List<string>(text.Split(new char[] { '.', '?', '!'},StringSplitOptions.RemoveEmptyEntries)) ;
     
            textBox.IsEnabled = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            countSec++;
            timerTextBlock.Text = countSec.ToString();
            if (countSec % 60 == 0)
            {
                avgSymbInMinute += symbolsInMinute;
                symbolsInMinute = 0;
            }
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            currentSentences = new List<string>();
            int numberSymbols = 0;
            for(int i = 1; i <= countSentences; i++)
            {
                do
                {
                    rand = random.Next(0, allSentences.Count);
                } while (currentSentences.Contains(allSentences[rand])!=false);
                currentSentences.Add(allSentences[rand]);
                numberSymbols += allSentences[rand].Length;
            }

            isCorrect = true;
         
            progressBar.Value = 0;
            progressBar.Maximum = numberSymbols;
            avgSymbInMinute = 0.0;
            symbolsInMinute = 0;
            currentSentence = 0;
            textBlock.Text = currentSentences[currentSentence];
            currentSymbol = 0;
            countMistakes = 0;
            textBox.IsEnabled = true;
            mistakesTextBlock.Text = countMistakes.ToString();
          
            countSymbols = 0;
            countSec = 0;
          
         
            textBox.Clear();
            textBox.Focus();
        }
       private void ShowResult()
        {
            
            MessageBox.Show($"Time: {countSec} sec\nSpeed: {avgSymbInMinute} s/m\nMistakes: {countMistakes}\nSymbols: {countSymbols}","Finish",MessageBoxButton.OKCancel);
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
                countSentences =Convert.ToInt32(slider.Value);
     
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            isCaseSensitivity =  Convert.ToBoolean(((RadioButton)sender).Tag);
           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (textBox.Text.Length > 0 && textBox.Text.Length - 1 == currentSymbol && ((isCaseSensitivity==false  && Char.ToLower(currentSentences[currentSentence][currentSymbol]) == Char.ToLower(textBox.Text[textBox.Text.Length - 1])) ||(isCaseSensitivity==true && currentSentences[currentSentence][currentSymbol]== textBox.Text[textBox.Text.Length - 1])))
            {

                isCorrect = true;
                textBox.Foreground = Brushes.Black;
                countSymbols++;
                progressBar.Value++;
                symbolsInMinute++;

                if ((currentSymbol == (currentSentences[currentSentence].Length - 1)) && (currentSentence == currentSentences.Count - 1))
                {
                    timer.Stop();

                    if(countSec%60!=0 && countSec/60<1)
                    avgSymbInMinute += symbolsInMinute;
                    if (countSec / 60 > 0)
                        avgSymbInMinute /= (countSec / 60);
                    ShowResult();

                    textBox.IsEnabled = false;

                }
                else if ((currentSymbol == (currentSentences[currentSentence].Length - 1)) && currentSentence < currentSentences.Count - 1)
                {
                    currentSentence++;
                    textBlock.Text += "\n" + currentSentences[currentSentence];
                    textBox.Clear();
                    currentSymbol = 0;


                }
                else
                {
                    currentSymbol++;
                    
                }
            }
            else if (isCorrect && textBox.Text.Length>0)
            {

                countMistakes++;
                mistakesTextBlock.Text = countMistakes.ToString();
                textBox.Foreground = Brushes.IndianRed;
                isCorrect = false;

            }
           
        }
    }
}
