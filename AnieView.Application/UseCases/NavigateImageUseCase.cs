using AnieView.Core.Interfaces;

namespace AnieView.Application.UseCases;

public class NavigateImageUseCase
{
    private readonly INavigationService _navigationService;

    public NavigateImageUseCase(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public string? GetNextPath(string currentPath)
    {
        return _navigationService.GetNextFile(currentPath);
    }

    public string? GetPreviousPath(string currentPath)
    {
        return _navigationService.GetPreviousFile(currentPath);
    }
}
