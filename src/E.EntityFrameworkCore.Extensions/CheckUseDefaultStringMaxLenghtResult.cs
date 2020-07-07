namespace E
{
    /// <summary>
    /// 检查字符串类型字段长度限制返回结果
    /// </summary>
    public class CheckUseDefaultStringMaxLenghtResult
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CheckUseDefaultStringMaxLenghtResult()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="maxLength">最大长度</param>
        public CheckUseDefaultStringMaxLenghtResult(bool success, int? maxLength = null)
        {
            this.Success = success;
            this.MaxLength = maxLength;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 自定义长度
        /// </summary>
        public int? MaxLength { get; set; }
    }
}
