using CAE.DTO;
using System;
using System.Threading.Tasks;
using System.Windows;
namespace CAE.Demo
{
    /// <summary>
    /// Cancha.xaml 的交互逻辑
    /// </summary>
    public partial class Cancha : Window
    {
        public Cancha(Guid id)
        {
            Loaded += async (s, e) =>
            {
                await GetCancha(id);
            };
            Closing += Cancha_Closing;
            InitializeComponent();
        }

        private void Cancha_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Canceled = true;
        }
        bool Canceled = false;
        #region 获取残差数据
        public async Task<bool> GetCancha(Guid taskID)
        {
            var conn = new Connection();
            try
            {
                return await Task.Run(async ()=> {
                    await conn.Send(new 回传残差数据Request()
                    {
                        TaskID = taskID
                    });
                    Result<回传残差数据> result;
                    do
                    {
                        result = await conn.ReceiveMessage<回传残差数据>();
                        if (result == null || !result.Success)
                        {
                            VM.Instance.Alert = "获取残差数据失败";
                            conn.Dispose();
                            return false;
                        }
                        this.Dispatcher.Invoke(()=>{
                            tbx.AppendText(result.Data.Result);
                        });
                    } while (!Canceled && !result.Data.IsEnd);
                    return true;
                });
            }
            catch (Exception ex)
            {
                VM.Instance.Alert = "获取残差数据发生异常:" + ex.Message;
                return false;
            }
            finally
            {
                conn.Dispose();
            }
        }
        #endregion

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tbx.IsFocused)
            {
                return;
            }
            tbx.ScrollToEnd();
        }
    }
}
