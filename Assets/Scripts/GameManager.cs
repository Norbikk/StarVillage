using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Image workerImage;
    [SerializeField] private Image warriorImage;
    [SerializeField] private Image raidImage;
    [SerializeField] private Button createWorker;
    [SerializeField] private Button createWarrior;
    [SerializeField] private TimerScript harverstTimer;
    [SerializeField] private TimerScript eatingTimer;
    [SerializeField] private Text workerText;
    [SerializeField] private Text warriorText;
    [SerializeField] private Text mineralText;
    [SerializeField] private Text waveCounterText;
    [SerializeField] private Text enemyCountText;
    [SerializeField] private Text skipEnemyWaveText;

    [SerializeField] private AudioListener muteClick;
    [SerializeField] private AudioSource warriorCreated;
    [SerializeField] private AudioSource workerCreated;
    [SerializeField] private AudioSource resourceEatingSound;
    [SerializeField] private AudioSource mineralHarvestSound;
    [SerializeField] private AudioSource raidStart;
    [SerializeField] private AudioSource buttonClickSound;
    
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winnerScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Text[] gameOverMineralsText;
    [SerializeField] private Text[] gameOverWorkersText;
    [SerializeField] private Text[] gameOverWavesText;
    [SerializeField] private Text gameOverWarriorText;

    [SerializeField] private float workerCreateTime;
    [SerializeField] private float warriorCreateTime;
    [SerializeField] private float raidMaxTime;
    
    private bool _readyToCreateWorker;
    private bool _readyToCreateWarrior;
    
    private float _workerTimer = 2;
    private float _warriorTimer = 3;
    private float _raidTimer;
    
    private int _workerCount = 3;
    private int _warriorCount = 0;
    private int _mineralsCount = 10;
    
    private int _mineralsPerWorker = 1;
    private int _mineralsPerWarrior = 3;
    
    private int _workerCost = 5;
    private int _warriorCost = 9;
    
    private int _nextRaid;
    private int _raidIncrease = 2;
    private int _waveCount;
    private int _skipEnemyWave = 4;
    private void Start()
    {
        UpdateText();
        _raidTimer = raidMaxTime;
        _readyToCreateWarrior = true;
        _readyToCreateWorker = true;
    }
    private void Update()
    {
        ResourceUpdate();
        WorkerUpdate();
        WarriorUpdate();
        RaidUpdate();
        GameOver();
        WinScreen();
    }
    private void WorkerUpdate()
    {
        if (_mineralsCount >= _workerCost && _readyToCreateWorker)
        {
            createWorker.interactable = true;
        }
        else if(_mineralsCount <= _workerCost || !_readyToCreateWorker)
        {
            createWorker.interactable = false;
        }

        if (!_readyToCreateWorker)
        {
            if (_workerTimer <= workerCreateTime)
            {
                _workerTimer += Time.deltaTime;
                workerImage.fillAmount = _workerTimer / workerCreateTime;
            }
            else if (_workerTimer >= workerCreateTime)
            {
                createWorker.interactable = true;
                _workerCount += 1;
                workerCreated.Play();
                _readyToCreateWorker = true;
            }
        }
    }
    private void WarriorUpdate()
    {
        if (_mineralsCount >= _warriorCost && _readyToCreateWarrior)
        {
            createWarrior.interactable = true;
        }
        else if(_mineralsCount <= _warriorCost || !_readyToCreateWarrior)
        {
            createWarrior.interactable = false;
        }
        if (!_readyToCreateWarrior)
        {
            if (_warriorTimer <= warriorCreateTime)
            {
                _warriorTimer += Time.deltaTime;
                warriorImage.fillAmount = _warriorTimer / warriorCreateTime;
            }
            else if (_warriorTimer >= warriorCreateTime)
            {
                createWarrior.interactable = true;
                _warriorCount += 1;
                warriorCreated.Play();
                _readyToCreateWarrior = true;
            }
        }
    }
    private void RaidUpdate()
    {
        _raidTimer += Time.deltaTime;
        raidImage.fillAmount = _raidTimer / raidMaxTime;
        if (_raidTimer >= raidMaxTime)
        {
            if (_skipEnemyWave > 0)
            {
                _raidTimer = 0;
                _skipEnemyWave--;
                skipEnemyWaveText.text = $"Враги пойдут через {_skipEnemyWave}";
            }

            if (_skipEnemyWave == 0)
            {
                raidStart.Play();
                _raidTimer = 0;
                _warriorCount -= _nextRaid;
                _nextRaid += _raidIncrease;
                _waveCount++;
                waveCounterText.text = _waveCount.ToString();
                enemyCountText.text = _nextRaid.ToString();
                skipEnemyWaveText.text = "Враги пошли!";
            }
        }
    }
    private void ResourceUpdate()
    {
        if (harverstTimer.create)
        {
            _mineralsCount += _workerCount * _mineralsPerWorker;
            mineralHarvestSound.Play();
        }

        if (eatingTimer.create)
        {
            _mineralsCount -= _warriorCount * _mineralsPerWarrior;
            resourceEatingSound.Play();
        }
        UpdateText();
    }

    private void UpdateText()
    {
        workerText.text = _workerCount.ToString();
        warriorText.text = _warriorCount.ToString();
        mineralText.text = _mineralsCount.ToString();
    }

    private void GameOver()
    {
        if (_warriorCount < 0)
        {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
            gameOverMineralsText[0].text = $"Собрано ресурсов: {_mineralsCount}";
            gameOverWavesText[0].text = $"Пережито волн: {_waveCount-2}";
            gameOverWorkersText[0].text = $"Создано рабочих: {_workerCount}";
        }
    }

    private void WinScreen()
    {
        if (_workerCount == 100 || _waveCount == 11 && _warriorCount >= 0)
        {
            Time.timeScale = 0;
            winnerScreen.SetActive(true);
            gameOverMineralsText[1].text = $"Собрано ресурсов: {_mineralsCount}";
            gameOverWavesText[1].text = $"Пережито волн: {_waveCount-2}";
            gameOverWorkersText[1].text = $"Создано рабочих: {_workerCount}";
            gameOverWarriorText.text = $"Создано войнов: {_warriorCount}";
        }
    }
    public void CreateWarrior()
    {
        _mineralsCount -= _warriorCost;
        _warriorTimer = 0;
        createWarrior.interactable = false;
        _readyToCreateWarrior = false;
        buttonClickSound.Play();
    }
    public void CreateWorker()
    {
        _mineralsCount -= _workerCost;
        _workerTimer = 0;
        createWorker.interactable = false;
        _readyToCreateWorker = false;
        buttonClickSound.Play();
    }

    public void Pause()
    {
        if (Time.timeScale > 0)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            buttonClickSound.Play();
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            buttonClickSound.Play();
        }
    }

    public void MuteOnClick()
    {
        muteClick.enabled = !muteClick.enabled;
    }
}
