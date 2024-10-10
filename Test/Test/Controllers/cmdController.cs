using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cmdController : ControllerBase
    {
        [HttpGet("stream")]
        public async Task StreamGitStatus(CancellationToken cancellationToken)
        {
            Response.ContentType = "text/event-stream";

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (TcpClient client = new TcpClient("192.168.100.123", 8899))
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            string responseData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            await Response.WriteAsync($"data: {responseData}\n\n");
                            await Response.Body.FlushAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Task.Delay(5000);
                }
            }
        }
    }

}
