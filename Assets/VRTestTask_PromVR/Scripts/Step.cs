using UnityEngine;
using UnityEngine.Events;

public enum StepStatus { NotStarted, CompletedCorrect, CompletedWithError, Skipped }

public abstract class Step : MonoBehaviour
{
    public string description;           // �������� ���� 
    public StepStatus status = StepStatus.NotStarted;
    public int stepIndex;                // ���������� ����� ���� � �������� 
    public delegate void StepCompleted(Step step);
    public event StepCompleted OnStepCompleted;

    public delegate void StepActivated(Step step);
    public event StepActivated OnStepActivated;
    protected bool isActiveStep = false; // ������� �� ������ ���� ���

    // ���������� ���������� , ����� ��� ���������� �������
    public virtual void ActivateStep()
    {
        isActiveStep = true;
        OnStepActivated?.Invoke(this);

    }

    // ���������� ���� (���������� �� ���������� ��� ���������� ��������)
    protected void CompleteStep(bool withError)
    {
        if (!isActiveStep)
        {
            // ���� ��� �� �������, �� ������ Complete (������������� ����������)
            // ��� �� �����, ������� ������ � ������� ���������
        }
        isActiveStep = false;
        
        // ����������� ������ ���� (��������� ��� � �������)
        status = withError ? StepStatus.CompletedWithError : StepStatus.CompletedCorrect;
        // ������� ���������� ����
        OnStepCompleted?.Invoke(this);
    }
}
