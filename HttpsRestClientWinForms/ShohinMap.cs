using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HttpsRestClientWinForms
{
    /// <summary>JSONデータ格納クラス</summary>
    /// <remarks></remarks>
    [Serializable]
    public class ShohinMap
    {
        [JsonPropertyName("numId")]
        public int NumId { get; set; }
        [JsonPropertyName("shohinCode")]
        public short ShohinCode { get; set; }
        [JsonPropertyName("shohinName")]
        public string ShohinName { get; set; }
        [JsonPropertyName("editDate")]
        public decimal EditDate { get; set; }
        [JsonPropertyName("editTime")]
        public decimal EditTime { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
}