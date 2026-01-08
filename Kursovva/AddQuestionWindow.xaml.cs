using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32; 
using System.IO;       
using Kursovva.Data;
using Kursovva.Models;

namespace Kursovva
{
    public partial class AddQuestionWindow : Window
    {
        private int _examId;
        private byte[] _currentImageBytes = null; 

        public AddQuestionWindow(int examId)
        {
            InitializeComponent();
            _examId = examId;
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png, *.jpeg)|*.jpg;*.png;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _currentImageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_currentImageBytes);
                    bitmap.EndInit();
                    imgPreview.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при завантаженні фото: " + ex.Message);
                }
            }
        }

        private void BtnClearImage_Click(object sender, RoutedEventArgs e)
        {
            _currentImageBytes = null;
            imgPreview.Source = null;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show("Введіть текст питання!");
                return;
            }

            int correctCount = 0;
            if (cbCorrect1.IsChecked == true) correctCount++;
            if (cbCorrect2.IsChecked == true) correctCount++;
            if (cbCorrect3.IsChecked == true) correctCount++;
            if (cbCorrect4.IsChecked == true) correctCount++;

            if (correctCount == 0)
            {
                MessageBox.Show("Позначте хоча б одну правильну відповідь!");
                return;
            }

            string type = correctCount > 1 ? "Multi" : "Single";

            using (var db = new AppDbContext())
            {
                var q = new Question
                {
                    Text = txtQuestion.Text,
                    ExamId = _examId,
                    Type = type, 
                    ImageData = _currentImageBytes
                };

                db.Questions.Add(q);
                db.SaveChanges();
                var answers = new List<Answer>
                {
                    new Answer { Text = ans1.Text, IsCorrect = cbCorrect1.IsChecked == true, QuestionId = q.Id },
                    new Answer { Text = ans2.Text, IsCorrect = cbCorrect2.IsChecked == true, QuestionId = q.Id },
                    new Answer { Text = ans3.Text, IsCorrect = cbCorrect3.IsChecked == true, QuestionId = q.Id },
                    new Answer { Text = ans4.Text, IsCorrect = cbCorrect4.IsChecked == true, QuestionId = q.Id }
                };

                foreach (var a in answers)
                {
                    if (!string.IsNullOrWhiteSpace(a.Text)) db.Answers.Add(a);
                }
                db.SaveChanges();
            }
            this.Close();
        }
    }
}