# README

# Step 1: 타입 생성

- EnityData를 상속받는 커스텀 클래스를 작성합니다.
- 직렬화를 위해 [Serializable] 특성을 달아줍니다.

``` C#
    [Serializable]
    public class ActorData : EntityData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
    }
```

# Step 2: 데이터 작성

- 작업할 씬 내의 GameObject에 DataComponent를 부착합니다.
- 이제 [Step 1]에서 작성한 커스텀 클래스가 Entity Type 필드에 표시됩니다.
- 원하는 Type을 선택 후 데이터를 작성해 줍니다.

<div align="up">
  <img src ="https://github.com/user-attachments/assets/ac7cf7d1-d217-470d-9525-4fb60c4afdb6" width = "298">
  <img src ="https://github.com/user-attachments/assets/4d028196-b16b-4072-8c69-12960b8ef938">
</div>

# Step 3: 저장할 데이터 묶기

- 씬에 빈 Root 오브젝트를 생성하여 저장할 데이터들을 자식으로 이동시켜 줍니다.
- 원하는 Filed Editor 윈도우를 열어서 bake target에 Root를 할당합니다.
- Root에 DataComponent가 부착되어 있지 않다면 AddFieldData 버튼을 눌러 초기화 시켜줍니다.
- FieldData를 작성해 줍니다.

  <img src = "https://github.com/user-attachments/assets/e231e0db-60e4-4493-ad02-8f8db8834ed7" width = "298">  
  <img src = "https://github.com/user-attachments/assets/8b3c86cd-9bee-437b-8376-45b8d8c1264d" width = "440">



# Step 4: 파일 내보내기

- 모든 데이터 작업이 완료되었다면 SaveFile 버튼을 눌러 Json 파일 형식으로 저장합니다.

![image](https://github.com/user-attachments/assets/aae9514d-1a9e-41d7-a6c6-1449c885ef08)
