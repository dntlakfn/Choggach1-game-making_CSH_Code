using PSB_Lib.Dependencies;
using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using UnityEngine;

namespace Work.Scripts.UI
{
    public class CloudSpawner : MonoBehaviour
    {
        [Inject] private PoolManagerMono _poolManager;
        [SerializeField] private PoolItemSO cloudPool;
        [SerializeField] private float spawnDelay;
        [SerializeField] private int spawnMaxCount;
        private RectTransform _rectTrm;
        int cloudCount = 0;
        float timer;
        private bool isAwake;

        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            Bus<PushCloudAfterEvent>.OnEvent += CountCloud;
        }

        public void AwakeCloudSpawner()
        {
            isAwake = true;
            for(int _ = 0; _ < 7; _++)
            {
                SpawnCloud(true);
            }
        }

        private void OnDestroy()
        {
            Bus<PushCloudAfterEvent>.OnEvent -= CountCloud;
        }
        private void Update()
        {
            if (!isAwake) return;

            timer += Time.deltaTime;
            if (timer >= spawnDelay && cloudCount < spawnMaxCount)
            {
                SpawnCloud(false);
                timer = 0;
            }
        }

        private void SpawnCloud(bool isFirstSpawn)
        {
            var cloud = _poolManager.Pop<StageUICloud>(cloudPool);
            cloud.transform.SetParent(transform);
            if (isFirstSpawn)
            {
                cloud.GetRectTransform().anchoredPosition = new Vector2(Random.Range(-900f, 901), Random.Range(-4200, -1000));

            }
            else
            {
                cloud.GetRectTransform().anchoredPosition = new Vector2(Random.Range(-900f, 901), -4700);

            }
            cloud.goalYPos = 503;
        }

        private void CountCloud(PushCloudAfterEvent evt)
        {
            cloudCount--;
        }
    }

    
}