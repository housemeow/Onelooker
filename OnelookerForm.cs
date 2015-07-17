using Kelly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Onelooker
{
    public partial class OnelookerForm : Form
    {
        private const string ONELOOK_URL = "http://www.onelook.com/?w={0}&ls=a";
        private const string WORD_NET_WORD = "<b>&#9656; <i>.*?<br>";
        private const string FORMATTED_CSS_CONTENT = "<style>{0}</style>";
        private const string FORMATTED_VOCABULARY_CONTENT = "<div class=\"vocabulary\">{0}</div>";
        private const string NOT_FOUND = "<div class=\"notFound\">not found</div>";
        private const string SEARCHING = "Searching...";
        private const string FORMATTED_HEADER = "<div class=\"title\">{0}</div><hr/>";
        private const string SEARCHING_COMPLETE = "Done";

        private string _searchText;

        ClipboardManager clipboardManager;

        public OnelookerForm()
        {
            InitializeComponent();
            clipboardManager = ClipboardManager.GetInstance();
            clipboardManager.ClipboardUpdate += OnClipboardManagerUpdate;
        }

        private void OnClipboardManagerUpdate(string text)
        {
            this._searchText = text;
            ShowSearchingStatus();
            Search();
            ShowDoneStatus();
        }

        private void Search()
        {
            HttpRequester requester = new HttpRequester();
            string url = String.Format(ONELOOK_URL, this._searchText);
            string result = requester.Get(url);
            MatchCollection matches = Regex.Matches(result, WORD_NET_WORD);
            string words = String.Format(FORMATTED_HEADER, this._searchText);
            foreach (Match match in matches)
            {
                words += String.Format(FORMATTED_VOCABULARY_CONTENT, match.Value);
            }
            InternalResourceReader reader = new InternalResourceReader(Assembly.GetExecutingAssembly());
            string css = String.Format(FORMATTED_CSS_CONTENT, reader.ReadAllText("Onelooker.style.css"));
            words += css;
            if (matches.Count > 0)
                _webBrowser.DocumentText = words;
            else
                _webBrowser.DocumentText = words + NOT_FOUND;
        }

        private void ShowSearchingStatus()
        {
            _toolStripStatusLabel.Text = SEARCHING;
            Refresh();
        }

        private void ShowDoneStatus()
        {
            _toolStripStatusLabel.Text = SEARCHING_COMPLETE;
            Refresh();
        }
    }
}
