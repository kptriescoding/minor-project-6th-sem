using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class JsonStructure{
    public string status;
    public string translation;
    public string text_summaries;
    public string text_long_answers;
    public string text_short_answers;
    public string text_questions;
    public string web_summaries;
    public string web_long_answers;
    public string web_short_answers;

}

public class LLMManager : MonoBehaviour
{
    private string engPageContent;
    [SerializeField]  private TMP_Text pageContent;
    [SerializeField]  private TMP_InputField searchInput;

    [SerializeField]  private TMP_Dropdown questionMode;

    [SerializeField]  private TMP_Dropdown mode;

    [SerializeField]  private TMP_Dropdown language;

    private JsonStructure pageContentJsonStructure=new JsonStructure();

    private string url="http://192.168.0.119:5000";

    private JsonStructure resJsonStructure;

    private string textBook="William Stallings - Operating Systems.pdf";
    
    // Start is called before the first frame update
    void Start()
    {
        url="http://192.168.56.162:5000";
        engPageContent = pageContent.text;
        questionModeChanged();
        StartCoroutine(setTextBook());
    }
    IEnumerator setTextBook(){
        string postLink = url+"/set_textbook";
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("textbook",textBook));
        yield return postRequest(postLink,form);
        if(resJsonStructure.status == "Success")
        {
            Debug.Log("Textbook set");
        }
        else{
            Debug.Log("Error in setting textbook");
        }
        resJsonStructure=null;

    }

    public void goToNextPage()
    {
        if(pageContent.pageToDisplay == pageContent.textInfo.pageCount)
        return;
        pageContent.pageToDisplay++;
    }
    public void goToPreviousPage()
    {
        if(pageContent.pageToDisplay == 1)
        return;
        pageContent.pageToDisplay--;
    }
    public void modeChanged(){
        questionModeChanged();
    }

    public void questionModeChanged(){
        pageContent.pageToDisplay=1;
        if(questionMode.value == 0)
        {
            if(mode.value == 0 && pageContentJsonStructure.text_summaries!=null)
            {
                pageContent.text = pageContentJsonStructure.text_summaries;
            }
            else if(mode.value == 1 && pageContentJsonStructure.web_summaries!=null)
            {
                pageContent.text = pageContentJsonStructure.web_summaries;
            }
            else{
                pageContent.text = "Click on the search button to get the summary";
            }
        }
        else if(questionMode.value==1){
            if(mode.value == 0 && pageContentJsonStructure.text_long_answers!=null)
            {
                pageContent.text = pageContentJsonStructure.text_long_answers;
            }
            else if(mode.value == 1 && pageContentJsonStructure.web_long_answers!=null)
            {
                pageContent.text = pageContentJsonStructure.web_long_answers;
            }
            else {
                pageContent.text = "Click on the search button to get the long answers";
            }
        }
        else if(questionMode.value==2){
            if(mode.value == 0 && pageContentJsonStructure.text_short_answers!=null)
            {
                pageContent.text = pageContentJsonStructure.text_short_answers;
            }
            else if(mode.value == 1 && pageContentJsonStructure.web_short_answers!=null)
            {
                pageContent.text = pageContentJsonStructure.web_short_answers;
            }
            else{
                pageContent.text = "Click on the search button to get the short answers";
            }
        }
        else if(questionMode.value==3){
            mode.value = 0;
        }
    }

    public void languageChanged(){
        if(language.value == 0)
        {
            pageContent.text = engPageContent;
        }
        else{
            try{
            StartCoroutine(translatePageContent());
            }
            catch{
                Debug.Log("Error in translation");
                pageContent.text = "Error in translation";
            }
        }
    }

    IEnumerator translatePageContent(){
        string postLink = url+"/get_translation";
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("text",engPageContent));
        form.Add(new MultipartFormDataSection("target_language",language.options[language.value].text));
        yield return postRequest(postLink,form);
        if(resJsonStructure.status == "Success")
        {
            pageContent.text = resJsonStructure.translation;
        }
        else{
            Debug.Log("Error in translation");
            pageContent.text = "Error in translation";
        }

        resJsonStructure=null;

  
    }
    IEnumerator postRequest(string postLink,List<IMultipartFormSection> form){

        // Post request in unity
        
       
        UnityWebRequest request = UnityWebRequest.Post(postLink,form);
        yield return request.SendWebRequest();
        try{
        string jsonRes=request.downloadHandler.text;
        print(jsonRes);
        resJsonStructure = JsonUtility.FromJson<JsonStructure>(jsonRes);
        }
        catch{
            Debug.Log("Error in post request");
            pageContent.text = "Error in post request";
        }
        
        // Access the data obtained through the request   
        
        
    }

    public void search()
    {
        if((mode.value==1||questionMode.value==1||questionMode.value==2)&&searchInput.text=="")
        {
            pageContent.text = "Please enter a question";
            return;
        }
        try{
        if(mode.value==0){
            if(questionMode.value==0){
                StartCoroutine(getTextSummaries());
            }
            else if(questionMode.value==1||questionMode.value==2) {
                StartCoroutine(getTextAnswers(searchInput.text));
            }
            else if(questionMode.value==3){
                StartCoroutine(getTextQuestions());
            }
        }
        else {
            StartCoroutine(getWebAnswers(searchInput.text));
        }
        }
        catch{
            Debug.Log("Error in post request");
            pageContent.text = "Error in post request";
        }
    }
    IEnumerator getTextSummaries(){
        string postLink = url+"/get_summary_for_textbook";
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        yield return postRequest(postLink,form);
        if(resJsonStructure.status == "Success")
        {
            pageContentJsonStructure.text_summaries = resJsonStructure.text_summaries;
            questionModeChanged();
        }
        else{
            Debug.Log("Error in getting summaries");
            pageContent.text = "Error in getting summaries";
        }  
    }
    IEnumerator getTextQuestions(){
        string postLink = url+"/get_question_for_textbook";
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        yield return postRequest(postLink,form);
        if(resJsonStructure.status == "Success")
        {
            pageContentJsonStructure.text_questions = resJsonStructure.text_questions;
            questionModeChanged();
        }
        else{
            Debug.Log("Error in getting questions");
            pageContent.text = "Error in getting questions";
        }
    }

    IEnumerator getTextAnswers(string question){
        string postLink = url+"/get_answer_for_textbook";
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("question",question));
        yield return postRequest(postLink,form);

        if(resJsonStructure.status == "Success")
        {
            pageContentJsonStructure.text_long_answers = resJsonStructure.text_long_answers;
            pageContentJsonStructure.text_short_answers = resJsonStructure.text_short_answers;
            questionModeChanged();
        }
        else{
            Debug.Log("Error in getting answers");
            pageContent.text = "Error in getting answers";
        }
        resJsonStructure=null;
    }

    IEnumerator getWebAnswers(string question){
        string postLink = url+"/do_web_search"; 
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("question",question));
        yield return postRequest(postLink,form);
        if(resJsonStructure.status == "Success")
        {
            pageContentJsonStructure.web_long_answers = resJsonStructure.web_long_answers;
            pageContentJsonStructure.web_short_answers = resJsonStructure.web_short_answers;
            pageContentJsonStructure.web_summaries = resJsonStructure.web_summaries;
            questionModeChanged();
        }
        else{
            Debug.Log("Error in getting  web answers");
            pageContent.text = "Error in getting web answers";
        }
        resJsonStructure=null;
    }


}
