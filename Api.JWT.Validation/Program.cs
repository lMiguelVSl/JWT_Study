using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var key = Guid.NewGuid().ToString(); //no good practice just to usage purpose
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(opt =>
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);

    opt.RequireHttpsMetadata = false;

    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signingKey
    };
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Hello World, protected").RequireAuthorization();
app.MapGet("/auth/{user}/{pass}", (string user, string pass) =>
{
    if (user != "Miguel" || pass != "MAS") return "Invalid User";

    var tokenHandler = new JwtSecurityTokenHandler();
    var byteKey = Encoding.UTF8.GetBytes(key);
    var tokenDes = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user)
        }),
        Expires = DateTime.UtcNow.AddMinutes(10),
        SigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDes);
    return tokenHandler.WriteToken(token);
});

app.Run();