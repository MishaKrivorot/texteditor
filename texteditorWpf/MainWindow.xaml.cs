// Підключення просторів імен
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace texteditorWpf
{
    public partial class MainWindow : Window
    {
        // Змінна для збереження шляху до поточного файлу
        private string currentFilePath = string.Empty;

        // Прапорець, що показує, чи були внесені зміни у документ
        private bool isModified = false;

        public MainWindow()
        {
            InitializeComponent();

            SetupShortcuts();      // Налаштування гарячих клавіш
            UpdateTitle();         // Оновлення заголовка вікна
        }

        // Метод налаштування команд та гарячих клавіш
        private void SetupShortcuts()
        {
            // Прив’язка стандартних команд до методів
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (s, e) => NewFile()));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (s, e) => OpenFile()));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (s, e) => SaveFile()));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, (s, e) => SaveFileAs()));

            // Додавання гарячих клавіш
            InputBindings.Add(new KeyBinding(ApplicationCommands.New, new KeyGesture(Key.N, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ApplicationCommands.Open, new KeyGesture(Key.O, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ApplicationCommands.Save, new KeyGesture(Key.S, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ApplicationCommands.SaveAs, new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)));
        }

        // Метод оновлення заголовка вікна
        private void UpdateTitle()
        {
            // Якщо файл ще не збережений — показуємо "Без назви"
            string fileName = string.IsNullOrEmpty(currentFilePath)
                ? "Без назви"
                : Path.GetFileName(currentFilePath);

            // Додаємо "*" якщо є незбережені зміни
            this.Title = isModified ? $"{fileName} *" : fileName;
        }

        // Обробник події зміни тексту
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            isModified = true; // Встановлюємо прапорець змін
            UpdateTitle();     // Оновлюємо заголовок
        }

        // Обробники пунктів меню (викликають відповідні методи)
        private void New_Click(object sender, RoutedEventArgs e) => NewFile();
        private void Open_Click(object sender, RoutedEventArgs e) => OpenFile();
        private void Save_Click(object sender, RoutedEventArgs e) => SaveFile();
        private void SaveAs_Click(object sender, RoutedEventArgs e) => SaveFileAs();
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        // Метод створення нового файлу
        private void NewFile()
        {
            // Перевірка необхідності збереження
            if (!CheckSaveBeforeAction()) return;

            textBox.Clear();           // Очищення текстового поля
            currentFilePath = string.Empty; // Скидання шляху файлу
            isModified = false;
            UpdateTitle();
        }

        // Метод відкриття файлу
        private void OpenFile()
        {
            // Перевірка необхідності збереження
            if (!CheckSaveBeforeAction()) return;

            // Створення діалогу відкриття файлу
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            // Якщо користувач вибрав файл
            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName; // Збереження шляху

                // Тимчасове відключення події TextChanged,
                // щоб не спрацьовував прапорець змін при завантаженні файлу
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.Text = File.ReadAllText(currentFilePath); // Зчитування тексту
                textBox.TextChanged += TextBox_TextChanged;

                isModified = false;
                UpdateTitle();
            }
        }

        // Метод збереження файлу
        private void SaveFile()
        {
            // Якщо файл ще не має шляху — викликаємо "Зберегти як"
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs();
                return;
            }

            // Запис тексту у файл
            File.WriteAllText(currentFilePath, textBox.Text);
            isModified = false;
            UpdateTitle();
        }

        // Метод "Зберегти як"
        private void SaveFileAs()
        {
            // Створення діалогу збереження файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            // Якщо користувач обрав шлях збереження
            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName; // Збереження нового шляху
                File.WriteAllText(currentFilePath, textBox.Text); // Запис тексту
                isModified = false;
                UpdateTitle();
            }
        }

        //Метод перевірки "Зберегти зміни?"
        private bool CheckSaveBeforeAction()
        {
            // Якщо змін не було — дозволяємо виконання дії
            if (!isModified) return true;

            // Виведення діалогового вікна підтвердження
            var result = MessageBox.Show(
                "Файл було змінено. Зберегти зміни?",
                "Підтвердження",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            // Якщо користувач обрав "Так"
            if (result == MessageBoxResult.Yes)
            {
                SaveFile();             // Виконуємо збереження
                return !isModified;     // Продовжуємо, якщо збереження успішне
            }
            // Якщо обрав "Ні"
            else if (result == MessageBoxResult.No)
            {
                return true;            // Продовжуємо без збереження
            }
            else
            {
                return false;           // Скасування дії
            }
        }

        // Обробник події закриття вікна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Якщо користувач скасував збереження — відміняємо закриття
            if (!CheckSaveBeforeAction())
                e.Cancel = true;
        }
    }
}
