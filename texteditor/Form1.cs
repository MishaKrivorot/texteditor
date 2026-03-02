using System;
using System.Windows.Forms;
using System.IO;

namespace texteditor
{
    public partial class Form1 : Form
    {
        // Змінна для збереження шляху до поточного файлу
        private string currentFilePath = string.Empty;

        // Прапорець, що вказує, чи були внесені зміни у документ
        private bool isModified = false;

        public Form1()
        {
            InitializeComponent();

            // Підписка на подію зміни тексту в RichTextBox
            richTextBox1.TextChanged += richTextBox1_TextChanged;

            // Підписка на подію закриття форми
            this.FormClosing += Form1_FormClosing;

            // Налаштування гарячих клавіш для пунктів меню
            this.newToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.N;
            this.openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            this.saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            this.saveasToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;

            UpdateTitle(); // Оновлення заголовка вікна
        }

        // Метод для оновлення заголовка вікна
        private void UpdateTitle()
        {
            // Якщо файл не збережений — показуємо "Без назви"
            string fileName = string.IsNullOrEmpty(currentFilePath)
                ? "Без назви"
                : Path.GetFileName(currentFilePath);

            // Якщо є незбережені зміни — додаємо "*"
            if (isModified)
                this.Text = fileName + " * ";
            else
                this.Text = fileName;
        }

        // Обробник події зміни тексту
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isModified = true;
            UpdateTitle();
        }

        // Обробник пункту меню "Новий"
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Перевірка необхідності збереження перед очищенням
            if (!CheckSaveBeforeAction()) return;

            richTextBox1.Clear();
            currentFilePath = string.Empty; // Скидання шляху файлу
            isModified = false;
            UpdateTitle();
        }

        // Обробник пункту меню "Відкрити"
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірка необхідності збереження перед відкриттям
            if (!CheckSaveBeforeAction()) return;

            // Створення діалогу відкриття файлу
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Якщо користувач вибрав файл
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName; // Збереження шляху
                richTextBox1.Text = File.ReadAllText(currentFilePath); // Зчитування тексту
                isModified = false;
                UpdateTitle();
            }
        }

        // Обробник пункту меню "Зберегти"
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(); // Виклик методу збереження
        }

        // Обробник пункту меню "Зберегти як"
        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs(); // Виклик методу "Зберегти як"
        }

        // Метод збереження файлу
        private void SaveFile()
        {
            // Якщо файл ще не має шляху — викликаємо "Зберегти як"
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileAs();
            }
            else
            {
                // Запис тексту у файл
                File.WriteAllText(currentFilePath, richTextBox1.Text);
                isModified = false;
                UpdateTitle();
            }
        }

        // Метод "Зберегти як"
        private void SaveFileAs()
        {
            // Створення діалогу збереження файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            // Якщо користувач вибрав місце збереження
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName; // Збереження нового шляху
                File.WriteAllText(currentFilePath, richTextBox1.Text); // Запис тексту
                isModified = false;
                UpdateTitle();
            }
        }

        // Обробник пункту меню "Вихід"
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Метод перевірки необхідності збереження перед виконанням дії
        private bool CheckSaveBeforeAction()
        {
            // Якщо змін не було — продовжуємо виконання
            if (!isModified) return true;

            // Виведення діалогового вікна підтвердження
            DialogResult result = MessageBox.Show(
                "Файл було змінено. Зберегти зміни?",
                "Підтвердження",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            // Якщо користувач обрав "Так"
            if (result == DialogResult.Yes)
            {
                SaveFile(); // Зберігаємо файл
                return true;
            }
            // Якщо обрав "Ні"
            else if (result == DialogResult.No)
            {
                return true; // Продовжуємо без збереження
            }
            else
            {
                return false; // Скасування дії
            }
        }

        // Обробник події закриття форми
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Якщо користувач скасував збереження — відміняємо закриття
            if (!CheckSaveBeforeAction())
            {
                e.Cancel = true;
            }
        }
    }
}