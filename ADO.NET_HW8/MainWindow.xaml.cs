using ADO.NET_HW8.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
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

namespace ADO.NET_HW8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Book book;
        private static Author author;
        private static Category category;
        private static Models.Publisher publisher;

        private static Book selectedBook;

        public MainWindow()
        {
            InitializeComponent();
            ShowBooks();
            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
        }

        private void Clear()
        {
            titleTxtBox.Clear();
            authorTxtBox.Clear();
            categoryTxtBox.Clear();
            publisherTxtBox.Clear();
            yearTxtBox.Clear();
            pagesTxtBox.Clear();
            selectedBook.Id = 0;
            deleteBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
            addBtn.IsEnabled = true;
            searchBtn.IsEnabled = true;
        }

        private void ShowBooks()
        {
            using (DBEntities db = new DBEntities())
            {
                dataGridView.ItemsSource = db.Book.ToList();
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titleTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(authorTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(categoryTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(publisherTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(yearTxtBox.Text) ||
                    string.IsNullOrWhiteSpace(pagesTxtBox.Text))
                {
                    MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля перед додаванням книги.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate that year and pages are valid integers
                else if (!int.TryParse(yearTxtBox.Text, out int year) || !int.TryParse(pagesTxtBox.Text, out int pages))
                {
                    MessageBox.Show("Рік видання та кількість сторінок повинні бути цілими числами.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (DBEntities db = new DBEntities())
                {
                    author = db.Author.SingleOrDefault(a => a.FullName == authorTxtBox.Text);
                    if (author == null)
                    {
                        author = new Author { FullName = authorTxtBox.Text };
                        db.Author.Add(author);
                    }

                    category = db.Category.SingleOrDefault(c => c.Name == categoryTxtBox.Text);
                    if (category == null)
                    {
                        category = new Category { Name = categoryTxtBox.Text };
                        db.Category.Add(category);
                    }

                    publisher = db.Publisher.SingleOrDefault(p => p.Name == publisherTxtBox.Text);
                    if (publisher == null)
                    {
                        publisher = new Models.Publisher { Name = publisherTxtBox.Text };
                        db.Publisher.Add(publisher);
                    }

                    book = new Book
                    {
                        Title = titleTxtBox.Text,
                        Authors = author,
                        Categories = category,
                        Publishers = publisher,
                        Year = int.Parse(yearTxtBox.Text),
                        Pages = int.Parse(pagesTxtBox.Text)
                    };
                    
                    db.Book.Add(book);
                    db.SaveChanges();
                }

                MessageBox.Show("Дані додано успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowBooks();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                Clear();
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(yearTxtBox.Text, out int year) || !int.TryParse(pagesTxtBox.Text, out int pages))
                {
                    MessageBox.Show("Рік видання та кількість сторінок повинні бути цілими числами.", "Ой", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (DBEntities db = new DBEntities())
                {
                    Book bookToUpdate = db.Book
                        .Include(b => b.Authors)
                        .Include(b => b.Categories)
                        .Include(b => b.Publishers)
                        .SingleOrDefault(b => b.Id == selectedBook.Id);

                    
                    bookToUpdate.Title = titleTxtBox.Text;
                    bookToUpdate.Year = year;
                    bookToUpdate.Pages = pages;

                    author = db.Author.SingleOrDefault(a => a.FullName == authorTxtBox.Text);
                    if (author == null)
                    {
                        author = new Author { FullName = authorTxtBox.Text };
                        db.Author.Add(author);
                    }
                    bookToUpdate.Authors = author;

                    category = db.Category.SingleOrDefault(c => c.Name == categoryTxtBox.Text);
                    if (category == null)
                    {
                        category = new Category { Name = categoryTxtBox.Text };
                        db.Category.Add(category);
                    }
                    bookToUpdate.Categories = category;

                    publisher = db.Publisher.SingleOrDefault(p => p.Name == publisherTxtBox.Text);
                    if (publisher == null)
                    {
                        publisher = new Models.Publisher { Name = publisherTxtBox.Text };
                        db.Publisher.Add(publisher);
                    }
                    bookToUpdate.Publishers = publisher;

                    db.SaveChanges();
                }

                MessageBox.Show("Дані змінено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                Clear();
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Ви дійсно хочете видалити цю книгу?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        Book bookToDelete = db.Book
                                .Include(b => b.Authors)
                                .Include(b => b.Categories)
                                .Include(b => b.Publishers)
                                .SingleOrDefault(b => b.Id == selectedBook.Id);

                        db.Book.Remove(bookToDelete);
                        db.SaveChanges();
                    }

                    MessageBox.Show("Дані видалено успішно", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    ShowBooks();
                }
            }
            catch (Exception ex) 
            { 
                MessageBox.Show($"Помилка: {ex.Message}"); 
            }
            finally 
            { 
                Clear(); 
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title = titleTxtBox.Text;
                string author = authorTxtBox.Text;
                string category = categoryTxtBox.Text;
                string publisher = publisherTxtBox.Text;

                int? year = !string.IsNullOrWhiteSpace(yearTxtBox.Text)
                    ? int.Parse(yearTxtBox.Text) : (int?)null;

                using (DBEntities db = new DBEntities())
                {
                    IQueryable<Book> query = db.Book
                        .Include(b => b.Authors)
                        .Include(b => b.Categories)
                        .Include(b => b.Publishers)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(title))
                        query = query.Where(b => b.Title.Contains(title));

                    if (!string.IsNullOrEmpty(author))
                        query = query.Where(b => b.Authors.FullName.Contains(author));

                    if (!string.IsNullOrEmpty(category))
                        query = query.Where(b => b.Categories.Name.Contains(category));

                    if (!string.IsNullOrEmpty(publisher))
                        query = query.Where(b => b.Publishers.Name.Contains(publisher));

                    if (year.HasValue)
                        query = query.Where(b => b.Year == year.Value);

                    List<Book> searchResults = query.ToList();
                    dataGridView.ItemsSource = searchResults;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void dataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridView.CurrentItem is Book book)
                {
                    using (var db = new DBEntities())
                    {
                        // Загрузка книги с учетом связанных сущностей
                        selectedBook = db.Book
                                     .Include(b => b.Authors)
                                     .Include(b => b.Categories)
                                     .Include(b => b.Publishers)
                                     .SingleOrDefault(b => b.Id == book.Id);

                        if (selectedBook != null)
                        {
                            titleTxtBox.Text = selectedBook.Title;
                            authorTxtBox.Text = selectedBook.Authors?.FullName;
                            categoryTxtBox.Text = selectedBook.Categories?.Name;
                            publisherTxtBox.Text = selectedBook.Publishers?.Name;
                            yearTxtBox.Text = selectedBook.Year.ToString();
                            pagesTxtBox.Text = selectedBook.Pages.ToString();

                            updateBtn.IsEnabled = true;
                            deleteBtn.IsEnabled = true;
                            addBtn.IsEnabled = false;
                            searchBtn.IsEnabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
    }
}
