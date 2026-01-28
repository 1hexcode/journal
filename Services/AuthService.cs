using Journal.Models;

namespace Journal.Services;

public class AuthService
{
    private readonly UserService _userService;
    private User _currentUser;

    public AuthService(UserService userService)
    {
        _userService = userService;
    }

    // Get current logged in user
    public User CurrentUser => _currentUser;
    
    // Check if user is logged in
    public bool IsAuthenticated => _currentUser != null;

    // Login
    public async Task<bool> Login(string email, string password)
    {
        var user = await _userService.GetUserByUserEmail(email);
        Console.WriteLine($"User found: {user?.Email}");
        Console.WriteLine($"Password match: {user?.Password == password}");
        if (user == null)
            return false;
        
        if (user.Password != password)
            return false;
        
        _currentUser = user;
        return true;
    }

    // Logout
    public void Logout()
    {
        Preferences.Remove("email");
        Preferences.Clear();
        _currentUser = null;
    }
    
}