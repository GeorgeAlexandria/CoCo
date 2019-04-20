using System.Collections.ObjectModel;
using System.ComponentModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class GeneralLanguageViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _quickInfoStates;
        private readonly ObservableCollection<string> _editorStates;

        public GeneralLanguageViewModel(GeneralLanguage language)
        {
            Language = language.Name;

            _quickInfoStates = new ObservableCollection<string>(QuickInfoStateService.SupportedStateByNames.Keys);
            _editorStates = new ObservableCollection<string>(EditorStateService.SupportedStateByNames.Keys);

            _selectedQuickInfoState = QuickInfoStateService.SupportedState[language.QuickInfoState];
            _selectedEditorState = EditorStateService.SupportedState[language.EditorState];

            QuickInfoStates = _quickInfoStates.GetDefaultListView();
            EditorStates = _editorStates.GetDefaultListView();
        }

        public string Language { get; }

        public ICollectionView QuickInfoStates { get; }

        public ICollectionView EditorStates { get; }

        private string _selectedQuickInfoState;

        public string SelectedQuickInfoState
        {
            get
            {
                if (_selectedQuickInfoState is null && QuickInfoStates.MoveCurrentToFirst())
                {
                    SelectedQuickInfoState = (string)QuickInfoStates.CurrentItem;
                }
                return _selectedQuickInfoState;
            }
            set => SetProperty(ref _selectedQuickInfoState, value);
        }

        private string _selectedEditorState;

        public string SelectedEditorState
        {
            get
            {
                if (_selectedEditorState is null && EditorStates.MoveCurrentToFirst())
                {
                    SelectedEditorState = (string)EditorStates.CurrentItem;
                }
                return _selectedEditorState;
            }
            set => SetProperty(ref _selectedEditorState, value);
        }

        public GeneralLanguage ExtractData() => new GeneralLanguage(Language)
        {
            QuickInfoState = QuickInfoStateService.SupportedStateByNames[SelectedQuickInfoState],
            EditorState = EditorStateService.SupportedStateByNames[SelectedEditorState],
        };
    }
}