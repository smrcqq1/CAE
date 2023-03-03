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
            InitializeComponent();
            DataContext = VM.Instance;
            Loaded += async (s,e) =>
            {
                await GetCancha(id);
            };
        }
        #region 获取残差数据
        public async Task<bool> GetCancha(Guid taskID)
        {
            VM.Instance.Cancha = "";
            var conn = new Connection();
            try
            {
                await conn.Send(new 回传残差数据Request()
                {
                    TaskID = taskID
                });
                var result = await conn.ReceiveMessage<回传残差数据>();
                if (result == null || !result.Success)
                {
                    VM.Instance.Alert = "获取残差数据失败:" + result.Message;
                    return false;
                }
                VM.Instance.Cancha += result.Data.Result;
                if (result.Data.IsEnd)
                {
                    conn.Dispose();
                }
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
            return true;
        }
        #endregion

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            tbx.ScrollToEnd();
        }
    }
}
