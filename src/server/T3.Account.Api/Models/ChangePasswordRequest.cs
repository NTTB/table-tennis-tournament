namespace T3.Account.Api.Models;

public record ChangePasswordRequest(string Username, string OldPassword, string NewPassword);