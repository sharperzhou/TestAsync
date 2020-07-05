using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Test4
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                if (value == _progress) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        private Visibility _progressVisibility = Visibility.Hidden;
        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set
            {
                if (value == _progressVisibility) return;
                _progressVisibility = value;
                OnPropertyChanged();
            }
        }

        private int _httpMethodIndex;
        public int HttpMethodIndex
        {
            get => _httpMethodIndex;
            set
            {
                if (value == _httpMethodIndex) return;
                _httpMethodIndex = value;
                OnPropertyChanged();
            }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                if (value == _url) return;
                _url = value;
                OnPropertyChanged();

            }
        }

        private bool _isSaveAs;
        public bool IsSaveAs
        {
            get => _isSaveAs;
            set
            {
                if (_isSaveAs == value) return;
                _isSaveAs = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabledOfSend = true;
        public bool IsEnabledOfSend
        {
            get => _isEnabledOfSend;
            set
            {
                if (_isEnabledOfSend == value) return;
                _isEnabledOfSend = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabledOfCancel = false;
        public bool IsEnabledOfCancel
        {
            get => _isEnabledOfCancel;
            set
            {
                if (_isEnabledOfCancel == value) return;
                _isEnabledOfCancel = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, string>> _requestParams;
        public List<KeyValuePair<string, string>> RequestParams
        {
            get => _requestParams;
            set
            {
                if (value == _requestParams) return;
                _requestParams = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<string, string>> _requestHeaders;
        public List<KeyValuePair<string, string>> RequestHeaders
        {
            get => _requestHeaders;
            set
            {
                if (value == _requestHeaders) return;
                _requestHeaders = value;
                OnPropertyChanged();
            }
        }

        private string _reponseContent;
        public string ReponseContent
        {
            get => _reponseContent;
            set
            {
                if (value == _reponseContent) return;
                _reponseContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendCommand => new DelegateCommand(
           async param =>
            {
                _cts = new CancellationTokenSource();
                HttpClientHelper.Handler.HttpReceiveProgress += Handler_HttpReceiveProgress;
                IsEnabledOfSend = false;
                IsEnabledOfCancel = true;
                ProgressVisibility = Visibility.Visible;

                try
                {
                    if (IsSaveAs)
                    {
                        await SaveAsFile(HttpMethodIndex);
                    }
                    else
                    {
                        await LoadToTextBox(HttpMethodIndex);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    IsEnabledOfSend = true;
                    IsEnabledOfCancel = false;
                    ProgressVisibility = Visibility.Hidden;
                    HttpClientHelper.Handler.HttpReceiveProgress -= Handler_HttpReceiveProgress;
                }

            });

        private void Handler_HttpReceiveProgress(object sender, System.Net.Http.Handlers.HttpProgressEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        public ICommand CancelCommand => new DelegateCommand(
            param => _cts?.Cancel());

        private async Task SaveAsFile(int option)
        {
            Task<Stream> streamTask;
            if (HttpMethodIndex == 0)
            {
                streamTask = HttpClientHelper.GetAsync(Url, RequestParams, RequestHeaders, _cts.Token);
            }
            else
            {
                streamTask = HttpClientHelper.PostAsync(Url, RequestParams, RequestHeaders, _cts.Token);
            }

            var dlg = new SaveFileDialog();
            if (!dlg.ShowDialog().GetValueOrDefault()) return;

            using (var fileStream = File.OpenWrite(dlg.FileName))
            using (var stream = await streamTask)
            {
                var bytesRead = 0;
                var buffer = new byte[4096];
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token)) > 0)
                {
                    await fileStream.WriteAsync(buffer.Take(bytesRead).ToArray(), 0, bytesRead, _cts.Token);
                }
            }
        }

        private async Task LoadToTextBox(int option)
        {
            if (HttpMethodIndex == 0)
                ReponseContent = await HttpClientHelper.GetStringAsync(
                    Url, RequestParams, RequestHeaders, _cts.Token);
            else
                ReponseContent = await HttpClientHelper.PostStringAsync(
                    Url, RequestParams, RequestHeaders, _cts.Token);
        }

        private CancellationTokenSource _cts;
    }
}
