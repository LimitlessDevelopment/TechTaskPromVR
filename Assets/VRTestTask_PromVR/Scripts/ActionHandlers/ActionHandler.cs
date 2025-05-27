using UnityEngine;
using UnityEngine.Events;

public enum ActionStatus { NotStarted, CompletedCorrect, CompletedWithError, Skipped }

public abstract class ActionHandler : MonoBehaviour
{
    public string description;           // �������� ���� 
    public ActionStatus status = ActionStatus.NotStarted;
    public int ActionIndex;                // ���������� ����� ���� � �������� 
    public delegate void ActionCompleted(ActionHandler action);
    public event ActionCompleted OnActionCompleted;

    public delegate void ActionActivated(ActionHandler action);
    public event ActionActivated OnActionActivated;
    protected bool isActiveAction = false; // ������� �� ������ ���� ���

    // ���������� ���������� , ����� ��� ���������� �������
    public virtual void ActivateStep()
    {
        isActiveAction = true;
        OnActionActivated?.Invoke(this);

    }

    // ���������� ���� (���������� �� ���������� ��� ���������� ��������)
    protected void CompleteAction(bool withError)
    {
        if (!isActiveAction)
        {
            // ���� ��� �� �������, �� ������ Complete (������������� ����������)
            // ��� �� �����, ������� ������ � ������� ���������
        }
        isActiveAction = false;
        
        // ����������� ������ ���� (��������� ��� � �������)
        status = withError ? ActionStatus.CompletedWithError : ActionStatus.CompletedCorrect;
        Debug.Log(status);
        // ������� ���������� ����
        OnActionCompleted?.Invoke(this);
    }
}
