using System;
using System.Collections.Generic;

namespace boop
{
    public class UIRegistry
    {
        private Dictionary<Type, IUIElement> _uiElements = new();
        private Stack<IUIElement> _uiElementStack = new();

        public void Register<T>(T uiElement) where T : IUIElement
        {
            _uiElements[typeof(T)] = uiElement;
            uiElement.Hide();
        }

        public void Unregister<T>(T uiElement) where T : IUIElement
        {
            _uiElements.Remove(uiElement.GetType());
        }

        public void Show(Type type)
        {
            if (!_uiElements.TryGetValue(type, out IUIElement uiElement)) return;

            if (_uiElementStack.Count > 0)
                _uiElementStack.Peek().Hide();
            _uiElementStack.Push(uiElement);

            uiElement.Show();
        }

        public void Hide(Type type)
        {
            if (!_uiElements.TryGetValue(type, out IUIElement uiElement)) return;

            if (_uiElementStack.Count > 0 && _uiElementStack.Peek() == uiElement)
                _uiElementStack.Pop();

            uiElement.Hide();
        }

        public void Back()
        {
            if (_uiElementStack.Count == 0) return;

            _uiElementStack.Pop().Hide();

            if (_uiElementStack.Count > 0)
                _uiElementStack.Peek().Show();
        }

        public void HideAll()
        {
            while (_uiElementStack.Count > 0)
                _uiElementStack.Pop().Hide();
        }
    }
}
