using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace playfair
{
    public partial class Form1 : Form
    {
        static int matrixSize = 5;
        static char[,] matrix = new char[matrixSize, matrixSize];

        public Form1()
        {
            InitializeComponent();
            textBoxKey.TextChanged += new EventHandler(textBoxKey_TextChanged); // Gán sự kiện TextChanged cho textBoxKey
        }

        // Khi nhấn nút Encrypt
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            string key = textBoxKey.Text;
            string plaintext = richTextBoxPlaintext.Text;

            GenerateMatrix(key);
            DisplayMatrix();
            string ciphertext = Encrypt(plaintext);

            // Hiển thị kết quả mã hóa trong cửa sổ mới
            ShowResultForm("Encrypted Text", ciphertext);
        }

        // Khi nhấn nút Decrypt
        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            string key = textBoxKey.Text;
            string ciphertext = richTextBoxPlaintext.Text;

            GenerateMatrix(key);
            DisplayMatrix();
            string decryptedText = Decrypt(ciphertext);

            // Hiển thị kết quả giải mã trong cửa sổ mới
            ShowResultForm("Decrypted Text", decryptedText);
        }

        // Hiển thị kết quả trong cửa sổ mới
        private void ShowResultForm(string title, string result)
        {
            ResultForm resultForm = new ResultForm(result);
            resultForm.Show();
        }

        // Xử lý sự kiện TextChanged của textBoxKey
        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            // Lưu vị trí con trỏ trước khi thay đổi
            int selectionStart = textBoxKey.SelectionStart;

            // Chuyển tất cả các ký tự trong textBoxKey thành chữ hoa
            textBoxKey.Text = textBoxKey.Text.ToUpper();

            // Đặt lại vị trí con trỏ sau khi thay đổi
            textBoxKey.SelectionStart = selectionStart;
        }

        // Tạo ma trận Playfair từ khóa
        private void GenerateMatrix(string key)
        {
            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; // Gộp I và J lại
            bool[] used = new bool[26];
            key = key.ToUpper().Replace("J", "I");

            int row = 0, col = 0;

            // Điền khóa vào ma trận
            foreach (char c in key)
            {
                if (!used[c - 'A'])
                {
                    matrix[row, col] = c;
                    used[c - 'A'] = true;
                    col++;
                    if (col == matrixSize)
                    {
                        col = 0;
                        row++;
                    }
                }
            }

            // Điền các ký tự còn lại của bảng chữ cái
            foreach (char c in alphabet)
            {
                if (!used[c - 'A'])
                {
                    matrix[row, col] = c;
                    col++;
                    if (col == matrixSize)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
        }

        // Hiển thị ma trận trong DataGridView
        private void DisplayMatrix()
        {
            dataGridViewMatrix.ColumnCount = matrixSize;
            dataGridViewMatrix.RowCount = matrixSize;
            for (int row = 0; row < matrixSize; row++)
            {
                for (int col = 0; col < matrixSize; col++)
                {
                    dataGridViewMatrix[col, row].Value = matrix[row, col];
                }
            }
        }

        // Chuẩn bị văn bản
        private string PrepareText(string input)
        {
            input = input.ToUpper().Replace("J", "I");
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]))
                {
                    output.Append(input[i]);

                    // Kiểm tra nếu ký tự tiếp theo giống ký tự hiện tại
                    if (i < input.Length - 1 && input[i] == input[i + 1])
                    {
                        output.Append('X'); // Thêm 'X' vào giữa
                    }
                }
            }

            // Đảm bảo độ dài của output luôn chẵn
            if (output.Length % 2 != 0)
            {
                output.Append('X'); // Thêm 'X' nếu độ dài lẻ
            }

            return output.ToString();
        }

        // Mã hóa văn bản
        private string Encrypt(string plaintext)
        {
            plaintext = PrepareText(plaintext);
            StringBuilder ciphertext = new StringBuilder();

            for (int i = 0; i < plaintext.Length; i += 2)
            {
                char a = plaintext[i];
                char b = plaintext[i + 1];

                (int row1, int col1) = FindPosition(a);
                (int row2, int col2) = FindPosition(b);

                if (row1 == row2)
                {
                    ciphertext.Append(matrix[row1, (col1 + 1) % matrixSize]);
                    ciphertext.Append(matrix[row2, (col2 + 1) % matrixSize]);
                }
                else if (col1 == col2)
                {
                    ciphertext.Append(matrix[(row1 + 1) % matrixSize, col1]);
                    ciphertext.Append(matrix[(row2 + 1) % matrixSize, col2]);
                }
                else
                {
                    ciphertext.Append(matrix[row1, col2]);
                    ciphertext.Append(matrix[row2, col1]);
                }
            }

            return ciphertext.ToString();
        }

        // Giải mã văn bản
        private string Decrypt(string ciphertext)
        {
            ciphertext = PrepareText(ciphertext);
            StringBuilder plaintext = new StringBuilder();

            for (int i = 0; i < ciphertext.Length; i += 2)
            {
                char a = ciphertext[i];
                char b = ciphertext[i + 1];

                (int row1, int col1) = FindPosition(a);
                (int row2, int col2) = FindPosition(b);

                if (row1 == row2)
                {
                    plaintext.Append(matrix[row1, (col1 - 1 + matrixSize) % matrixSize]);
                    plaintext.Append(matrix[row2, (col2 - 1 + matrixSize) % matrixSize]);
                }
                else if (col1 == col2)
                {
                    plaintext.Append(matrix[(row1 - 1 + matrixSize) % matrixSize, col1]);
                    plaintext.Append(matrix[(row2 - 1 + matrixSize) % matrixSize, col2]);
                }
                else
                {
                    plaintext.Append(matrix[row1, col2]);
                    plaintext.Append(matrix[row2, col1]);
                }
            }

            return RemoveExtraX(plaintext.ToString());
        }

        // Tìm vị trí của ký tự trong ma trận
        private (int, int) FindPosition(char c)
        {
            for (int row = 0; row < matrixSize; row++)
            {
                for (int col = 0; col < matrixSize; col++)
                {
                    if (matrix[row, col] == c)
                    {
                        return (row, col);
                    }
                }
            }
            throw new ArgumentException("Character not found in matrix");
        }

        // Hàm để loại bỏ ký tự 'X' không cần thiết
        private string RemoveExtraX(string text)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                // Nếu ký tự là 'X' và là ký tự cuối cùng hoặc giống ký tự trước đó thì bỏ qua
                if (text[i] == 'X' && (i == text.Length - 1 || text[i - 1] == text[i + 1]))
                {
                    continue;
                }
                output.Append(text[i]);
            }
            return output.ToString();
        }
    }
}
