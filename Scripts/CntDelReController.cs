using UnityEngine;
using TMPro;

public class CntDelReController : MonoBehaviour
{
    [SerializeField] private TMP_InputField tmp_input;
    private string inputStr;
    [SerializeField] private TMP_InputField tmp_output;
    private string outputStr;
    private string outputStrCopy;
    [SerializeField] private TMP_InputField tmp_devide;
    private string devideStr;
    [SerializeField] private TMP_InputField tmp_outputDevide;
    private string outputDevideStr;
    [SerializeField] private TMP_Dropdown tmp_modeDropdown;
    [SerializeField] private TextMeshProUGUI tmp_cnt;
    [SerializeField] private TextMeshProUGUI tmp_resultCnt;

    private string[] inputList;

    private void OnDisable()
    {
        tmp_input.text = "";
        tmp_output.text = "";
        tmp_devide.text = "";
        tmp_outputDevide.text = "";
        tmp_modeDropdown.value = 0;
    }
    public void outputBtn()
    {
        outputStr = new string("");
        outputStrCopy = new string("");
        inputStr = tmp_input.text;
        devideStr = tmp_devide.text == null ? "" : tmp_devide.text;
        outputDevideStr = tmp_outputDevide.text == null ? "" : tmp_outputDevide.text;

        tmp_cnt.text = inputStr.Length.ToString();

        if (devideStr.Equals("")) // 单个字符串 去除重复的字符
        {
            for (int i = 0; i < inputStr.Length; i++)
            {
                string inputStrTmp = inputStr.Substring(i, 1);
                if (outputStrCopy.IndexOf(inputStrTmp) == -1)
                {
                    outputStrCopy += inputStrTmp;
                    outputStr += inputStrTmp + outputDevideStr;
                }
            }
        }
        else // 多个字符串 去除重复的字符串
        {
            inputList = inputStr.Split(devideStr, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < inputList.Length; i++)
            {
                if (tmp_modeDropdown.value == 1) // 从所有字符中去重
                {
                    string inputListTmp = "";
                    for (int j = 0; j < inputList[i].Length; j++)
                    {
                        string inputStrTmp = inputList[i].Substring(j, 1);
                        if (outputStrCopy.IndexOf(inputStrTmp) == -1 && inputListTmp.IndexOf(inputStrTmp) == -1)
                        {
                            inputListTmp += inputStrTmp;
                        }
                    }
                    inputList[i] = inputListTmp;
                }
                else if (tmp_modeDropdown.value == 2) // 从每组inputList[i]中去重
                {
                    string inputListTmp = "";
                    for (int j = 0; j < inputList[i].Length; j++)
                    {
                        string inputStrTmp = inputList[i].Substring(j, 1);
                        if (inputListTmp.IndexOf(inputStrTmp) == -1)
                        {
                            inputListTmp += inputStrTmp;
                        }
                    }
                    inputList[i] = inputListTmp;
                }

                if (inputList[i] != null && !inputList[i].Equals(""))
                {
                    outputStrCopy += inputList[i];
                    outputStr += inputList[i] + outputDevideStr;
                }
                
            }
        }

        outputStr = outputStr.Remove(outputStr.Length - outputDevideStr.Length, outputDevideStr.Length);

        tmp_output.text = outputStr;
        tmp_resultCnt.text = outputStr.Length.ToString();
    }

}
