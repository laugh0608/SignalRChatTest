using SignalRChatTest.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// 注册 SignalR 中心所需的服务
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// 配置 SignalR 终结点，对应 chat.js 文件中的启动链接路径
// app.MapHub<ChatHub>("/Chat");
// 更换为强类型中心
app.MapHub<StronglyTypedChatHub>("/Chat");

app.Run();