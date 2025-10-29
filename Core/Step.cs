namespace VisualWorkflowBuilder.Core;

public sealed class Step
{
     private string actionName;
     private int id;
     

     
     
     
     public string ActionName
     {
          get { return actionName; }
          set { actionName = value; }
     }

     public int Id
     {
          get { return id; }
          set { id = value; }
     }

}