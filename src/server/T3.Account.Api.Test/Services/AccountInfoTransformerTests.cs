using System.Security.Claims;
using T3.Account.Api.Services;

namespace T3.Account.Api.Test.Services;

public class AccountInfoTransformerTests
{
   IAccountInfoTransformer _sut = new AccountInfoTransformer();

   [Test]
   public void Test()
   {
       var nameIdentifier = RandomData.NextNameIdentifier();
       var nextUsername = RandomData.NextUsername();
       var claimsIdentity = new ClaimsIdentity(new[]{
               new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
               new Claim(ClaimTypes.GivenName, nextUsername),
            
      });
      var result = _sut.FromClaimsPrincipal(new ClaimsPrincipal(claimsIdentity));
      
      Assert.Multiple(() =>
      {
          Assert.That(result.GivenName, Is.EqualTo(nextUsername));
          Assert.That(result.Subject, Is.EqualTo(nameIdentifier));
      });
      
   }
}