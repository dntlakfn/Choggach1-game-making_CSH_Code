using UnityEngine;
using UnityEngine.Rendering;

namespace Work.CSH.Code.Maps
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private SortingGroup sortingGroup;
        public void CreateWall(GameObject up, GameObject down, GameObject left, GameObject right)
        {
            
            for (int i = 0; i < 4; i++)
            {
                Instantiate(i switch
                {
                    0 => right,
                    1 => left,
                    2 => up,
                    3 => down,
                    _ => null
                }, transform.position + (i < 2 ? new Vector3(10 - (1+10*i), 2) : new Vector3(-1,2 - (10*(i-2)))), Quaternion.identity, transform);
            }
        }

        public void SetOrderInLayer(int orderNum)
        {
            sortingGroup.sortingOrder = orderNum;
        }
    }
}
