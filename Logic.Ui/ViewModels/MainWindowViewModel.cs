﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using De.HsFlensburg.ClientApp001.Logic.Ui.Wrapper;
using De.HsFlensburg.ClientApp001.Service.MessageBusWithParameter;
using De.HsFlensburg.ClientApp001.Service.MessageBusWithParameter.MessageBusMessages;
using De.HsFlensburg.ClientApp001.Service.MessageBusWithParameter.MessageBusWithParameterMessages;
using De.HsFlensburg.ClientApp001.Services.SerializationService;

namespace De.HsFlensburg.ClientApp001.Logic.Ui.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand DeleteSelectedModelCommand { get; }
        public ICommand OpenEditSelectedModelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand OpenNewBookWindowCommand { get; }
        public BookCollectionViewModel MyList { get; set; }
        private ModelFileHandler modelFileHandler;
        private string pathForSerialization;
        private BookViewModel selectedBook;

        public BookViewModel SelectedBook
        {
            get
            {
                return selectedBook;
            }
            set
            {
                selectedBook = value;
                OnPropertyChanged("SelectedBook");
            }
        }

        public MainWindowViewModel(BookCollectionViewModel viewModelCollection)
        {
            DeleteSelectedModelCommand = new RelayCommand(DeleteSelectedModelMethod);
            OpenEditSelectedModelCommand = new RelayCommand(OpenEditSelectedModelMethod);
            SaveCommand = new RelayCommand(SaveModel);
            LoadCommand = new RelayCommand(LoadModel);
            OpenNewBookWindowCommand = new RelayCommand(OpenNewBookWindowMethod);
            MyList = viewModelCollection;
            modelFileHandler = new ModelFileHandler();
            pathForSerialization = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                   "\\BookCollectionSerialization\\MyBooks.cc";
        }

        private void OpenEditSelectedModelMethod()
        {
            if (SelectedBook != null)
            {
                SendSelectedBook();
                Messenger.Instance.Send(new OpenEditBookWindowMessage());
            }
            else
            {
                Console.Out.WriteLine("Kein Buch ausgewählt");
            }
        }


        private void DeleteSelectedModelMethod()
        {
            if (SelectedBook != null)
            {
                 MyList.Remove(SelectedBook);
            }
        }

        private void SaveModel()
        {
            modelFileHandler.WriteModelToFile(pathForSerialization, MyList.Model);
        }

        private void LoadModel()
        {
            MyList.Model = modelFileHandler.ReadModelFromFile(pathForSerialization);
        }

        private void OpenNewBookWindowMethod()
        {
            Messenger.Instance.Send(new OpenNewBookWindowMessage());
        }

        private void SendSelectedBook()
        {
            Messenger.Instance.Send<BookViewModel>(SelectedBook);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}