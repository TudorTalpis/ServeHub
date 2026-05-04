using TWeb.Domain.Models;

namespace TWeb.BusinessLayer.Interfaces;

public interface IUserAction
{
    List<UserDto> GetAllUserAction();
    UserDto? GetUserByIdAction(string id);
    UserDto? UserLoginAction(LoginRequestDto dto);
    UserDto UserSignUpAction(SignUpRequestDto dto);
    UserDto? UpdateUserAction(string id, UpdateUserDto dto);
    bool ChangePasswordAction(string id, ChangePasswordDto dto);

    // Notification
    List<NotificationDto> GetAllNotificationAction();
    List<NotificationDto> GetByUserIdNotificationAction(string userId);
    NotificationDto CreateNotificationAction(CreateNotificationDto dto);
    bool MarkAsReadNotificationAction(string id);
    void MarkAllAsReadNotificationAction(string userId);
}

