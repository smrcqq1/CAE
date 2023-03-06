using CAE.DTO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CAE.Demo
{
    internal class Connection : IDisposable
    {
        TcpClient client = new TcpClient();

        public void Dispose()
        {
            client.Close();
        }
        NetworkStream stream;
        public async Task<bool> Send<TRequest>(TRequest request)  where TRequest : LoginedRequest
        {
            if (request == null)
            {
                VM.Instance.Alert = "不能发送空消息";
                return false;
            }
            if (!VM.Instance.Logined || VM.Instance.Userinfo == null || string.IsNullOrEmpty(VM.Instance.Userinfo.Token))
            {
                VM.Instance.Alert = "请先登录";
                return false;
            }
            request.Token = VM.Instance.Userinfo.Token;
            return await send(request);
        }

        async Task<bool> send(object request)
        {
            if (request == null)
            {
                VM.Instance.Alert = "不能发送空消息";
                return false;
            }
            if(client.Client == null)
            {
                client = new TcpClient();
            }
            if (!client.Connected)
            {
                try
                {
                    await client.ConnectAsync(VM.Instance.Userinfo.IP, VM.Instance.Userinfo.Port.Value);
                }
                catch (Exception)
                {
                    VM.Instance.Alert = "连接失败";
                    client.Close();
                    return false;
                }
            }
            stream = client.GetStream();
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var data = System.Text.Encoding.UTF8.GetBytes(str);
            var len = BitConverter.GetBytes(data.Length);
            try
            {
                await stream.WriteAsync(len, 0, len.Length);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception)
            {
                VM.Instance.Alert = "发送失败";
                client.Close();
                return false;
            }
            return true;
        }
        public async Task<bool> Send(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                VM.Instance.Alert = "不能发送空数据";
                return false;
            }
            if (client.Client == null)
            {
                client = new TcpClient();
            }
            if (!client.Connected)
            {
                try
                {
                    await client.ConnectAsync(VM.Instance.Userinfo.IP, VM.Instance.Userinfo.Port.Value);
                }
                catch (Exception)
                {
                    VM.Instance.Alert = "连接失败";
                    client.Close();
                    return false;
                }
            }
            stream = client.GetStream();
            try
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception)
            {
                VM.Instance.Alert = "发送失败";
                client.Close();
                return false;
            }
            return true;
        }
        public async Task<bool> Send(object request)
        {
            return await send(request);
        }
        public async Task<Result<T>> ReceiveMessage<T>()
        {
            try
            {
                if (!client.Connected || stream == null)
                {
                    return new Result<T>("请先建立连接");
                }
                var len = new byte[4];
                await stream.ReadAsync(len, 0, 4);
                var length = BitConverter.ToInt32(len, 0);
                var buffer = new byte[length];
                await stream.ReadAsync(buffer,0,length);
                var str = System.Text.Encoding.UTF8.GetString(buffer);
                VM.Instance.Message = str;
                if (string.IsNullOrEmpty(str))
                {
                    return new Result<T>("服务器端返回空字符串");
                }
                try
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<T>>(str);
                    return data;
                }
                catch (Exception)
                {
                    return new Result<T>("服务器返回的消息无法被正确序列化:" + str);
                }
            }
            catch (Exception ex)
            {
                return new Result<T>("接受消息发生异常:" + ex.Message);
            }
        }
        public async Task<CommonResult> ReceiveMessage()
        {
            try
            {
                if (!client.Connected || stream == null)
                {
                    return new CommonResult("请先建立连接");
                }
                var len = new byte[4];
                await stream.ReadAsync(len, 0, 4);
                var length = BitConverter.ToInt32(len, 0);
                var buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, length);
                var str = System.Text.Encoding.UTF8.GetString(buffer);
                VM.Instance.Message = str;
                if (string.IsNullOrEmpty(str))
                {
                    return new CommonResult("服务器端返回空字符串");
                }
                try
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonResult>(str);
                    return data;
                }
                catch (Exception)
                {
                    return new CommonResult("服务器返回的消息无法被正确序列化:" + str);
                }
            }
            catch (Exception ex)
            {
                return new CommonResult("接受消息发生异常:" + ex.Message);
            }
        }
        public async Task<byte[]> ReceiveData()
        {
            var len = await ReceiveData(4);
            var length = BitConverter.ToInt32(len, 0);
            return await ReceiveData(length);
        }
        public async Task<byte[]> ReceiveData(int length)
        {
            var buffer = new byte[length];
            await stream.ReadAsync(buffer, 0, length);
            return buffer;
        }
    }
}