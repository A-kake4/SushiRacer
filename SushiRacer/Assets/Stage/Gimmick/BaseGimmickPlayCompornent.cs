using UnityEngine;


public abstract class BaseGimmickPlayCompornent : MonoBehaviour
{
    public virtual void GimmickPlayTriggerEnter( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmickPlayTriggerExit( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmickPlayTriggerStay( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmickPlayCollisionEnter( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmickPlayCollisionExit( BaseGimmickMoveCompornent hitObject ) { }
    public virtual void GimmickPlayCollisionStay( BaseGimmickMoveCompornent hitObject ) { }
}
