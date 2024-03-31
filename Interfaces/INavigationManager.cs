using System;

namespace dev_flow.Interfaces;

public interface INavigationManager
{
    void GoForward();
    void GoBack();
    bool Navigate(Uri sourcePageUri, object extraData);
    bool Navigate(Type sourceType);
}