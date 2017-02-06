using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace promodz.Hardware
{
    class ProModzHardware
    {

        UInt32 CCS_magic = 0x94573759;
        int CCS_max_temp_power_graph_points = 7;

        public enum CCS_LED_STATE : byte
        {
            CCS_LED_STATE_OFF,
            CCS_LED_STATE_BLINK_SLOW,
            CCS_LED_STATE_BLINK_FAST,
            CCS_LED_STATE_ON
        };

        public enum CCS_ID_list : byte
        {
            // ->PC передаётся постоянно
            CCS_ID_capabilities,        // код структуры возможностей
            CCS_ID_pump_RPM,
            CCS_ID_fan_RPM,
            CCS_ID_flow_RPM,
            CCS_ID_temperature,
            CCS_ID_pump_channel_power,
            CCS_ID_fan_channel_power,
            CCS_ID_inputs,
            CCS_ID_outputs,
            // ->PC передаётся по запросу,
            CCS_ID_config,
            CCS_ID_description, // 0xa
            CCS_ID_fan_channel_assignment, // 0xb
            CCS_ID_thermo_power_pump_graph, // 0xc
            CCS_ID_thermo_power_fan_graph, // 0xd
            CCS_ID_thermo_color_graph,

            CCS_ID_END                  // (служебный!!) - для организации цикла по всем ID
        };

        // RGB цвет подсветки
        public struct CCS_RGB_LED
        {
            byte r, g, b;
        };
        // общая конфигурация
        // если значение выходит за пределы, контроллер его оганичит.
        public struct CCS_config
        {
            byte temperature_full_power;     // температура (на любом канале измерения) перехода на полную мощность в градусах цельсия
            byte temperature_buzzer;         // температура (на любом канале измерения) включения звука
            byte temperature_shutdown;       // температура (на любом канале измерения) критическая - переход к отключению - в градусах цельсия
            UInt16 flow_min_RPM;              // минимальные обороты расходомера воды - иначе отключение [0-10000] об/минуту
            UInt16 pump_min_RPM;              // минимальные обороты помпы - иначе отключение [0-10000] об/минуту
            UInt16 max_alarm_time_ms;         // время (мс) от момента детектирования аварийной ситуаци и до начала процедуры выключения компьютера
            UInt16 max_startup_time_ms;       // время (мс) от момента включения до разрешения детектирования аварийной ситуаци - для раскрутки помп и вентиляторов
            UInt16 shutdown_time_ms;          // время (мс) выдержки в выключенном состоянии
            UInt16 pump_switch_period_s;		// время (с) переключения между помпами при нормальной работе - для равномерного износа помп. (если помп несколько)
        };
        /*==================================================================================*/
        /* настройка кривых "температура на канале" -> "мощность на канале" вентиляторов
         * каждая кривая остоит из CCS_max_temp_power_graph_points
         * всего может быть не более CCS_max_temp_power_graphs графиков
         * контроллер не производит проверок на пересечение областей даных. так что заполнять эти поля внимательно !!
         * Если заданы несколько графиков для одного канала вентиляторов - сработает тот, что даст максимальную мощность.
         * Графики должны быть линейно-возрастающими !!!
         * */
        public struct CCS_termo_power_point
        {
            sbyte temperature;
            byte power;
        };
        public struct CCS_termo_power_graph
        {
            byte temperature_channel;
            CCS_termo_power_point[] power_points;
        };
        /*==================================================================================*/
        /* настройка кривых "температура на канале" -> "Цвет подсветки" вентиляторов
         * 
         * */
        //  одна точка на графике
        public struct CCS_termo_color_point
        {
            byte temperature;
            CCS_RGB_LED color;
        };

        public struct CCS_termo_color
        {
            byte temperature_channel;    // из какого канала брать температуру
            //uint8_t RGB_LED_channel;		// на какой канал подсветки выводить
            CCS_termo_color_point[] power_points;
        };

        /************************************************************************************/
        /***  Описание данных, передающихся от контролера к компьютеру       *********/
        /************************************************************************************/
        /* общие сведения о количестве периферии в контроллере - возможности контроллера
         * контроллер передаёт данную структуру в компьютерную программу.
         * */
        public struct CCS_capabilities
        {
            UInt32 magic;             // CCS_magic
            byte num_RGB_leds;       // количество свеотдиодов трёхцветных свеотдиодных лент
            byte num_fan_channels;   // количество каналов управления венитляторами
            byte num_fans;           // общее количество вентиляторов
            byte num_flow;           // количество измерителей потока
            byte num_leds;           // количество свеотдиодов на плате - для отображения состояния
            byte num_pumps;          // количество каналов управления насосами
            byte num_relays;         // количество реле
            byte num_temp;           // количество измерителей температуры
            UInt32[] serial;			// серийный номер процессора. и контроллера 4 
        };

        /*==================================================================================*/
        /*
         * с помощью данной структуры описывается соотношение: канал управления вентиляторами и сколько на канале самих вентиляторов
         * Данная структура повторяется в пакете несколько раз  - для каждого канала
         * контроллер передаёт данную структуру в компьютерную программу.
         * */
        public struct CCS_fan_channel
        {
            byte channel;            // количество каналов управления насосами
            byte num_fans;           // общее количество вентиляторов
        }
        /*==================================================================================*/
        /*
         * Различные входы контроллера
         * fluid_level - датчик уровня жидкости. Чем больше значение - тем более криитчный уровень. ( 0 - норма) (0xff - аварийный уровень)
         * input - кнопки побитово (опция)
         * adc[] - переменные резисторы на панели (опция) [0...255]
         * voltage[] - напряжение поканально (опция)
         * current[] - ток поканально (опция)
         * */
        public struct CCS_inputs
        {
            byte fluid_level;
            byte input;
            byte[] adc;
            UInt16[] voltage; // формат 8:8 бит в вольтах
            UInt16[] current; // формат 8:8 бит в амперах
        };
        /*==================================================================================*/
        /*
         * Различные выходы контроллера
         * */
        public struct CCS_outputs
        {
            byte buzzer;             // состояние звукового сигнализатора
            byte relay_power_button; // эмулятор нажатия кнопки питания компьютера
            byte relay_power_cut;    // реле аварийного отключения блока питания.
            byte relay_aux;          // дополнительное реле
        };
        /*==================================================================================
        #define CCS_max_pump			4
        #define CCS_max_fan				26
        #define CCS_max_fan_chn			12
        #define CCS_max_temp			12
        #define CCS_max_flow_meters		2
        #define CCS_max_led				8
        #define CCS_max_RGB_led			3
        #define CCS_max_temp_power_graph_points 7
        #define CCS_max_ADC		4
        #define CCS_max_voltage 4
        #define CCS_max_current 4
        #define CCS_max_relays  4

        #define CCS_max_memory_alloc		64		// максимальный размер передаваемой структуры в байтах

        // пределы допустимых значений температуры
        #define CCS_min_temperature -120
        #define CCS_max_temperature 120
        // magic ;)
        #define CCS_magic 0x94573759

        /*==================================================================================*/
        /*
         * структура передаётся контроллером в компьютер
         * тип содержимого определяется полем ID из CCS_IDx_list
         * 
         * внимание !!! размер структуры может быть меньше !!! передаются только значащие данные !!!
         * 
         * *
        typedef struct {

        uint8_t ID;
        union{
        // передаваемые постояно - за каждый интервал опроса USB передаётся одна структура
		CCS_capabilities capabilities;                      // [CCS_ID_capabilities] возможности контроллера (const для контроллера)
                                                            // Даная структура является признаком наличия контроллера
        uint16_t pump_RPM[CCS_max_pump];                    // [CCS_ID_pump_RPM] измеренные скорости вращения лопастей насосов
        uint16_t fan_RPM[CCS_max_fan];                      // [CCS_ID_fan_RPM] измеренные скорости вращения лопастей венитляторов
        uint16_t flow_RPM[CCS_max_flow_meters];             // [CCS_ID_flow_RPM] измеренные скорости вращения лопастей измерителей потока
        int8_t temperature[CCS_max_temp];                   // [CCS_ID_temperature] измеренные температуры ( в градусах цельсия )
        int8_t pump_channel_power[CCS_max_pump];            // [CCS_ID_pump_channel_power] текущие значения мощности на каналах насосов
        int8_t fan_channel_power[CCS_max_fan_chn];          // [CCS_ID_fan_channel_power] текущие значения мощности на каналах вентиляторов
        CCS_inputs inputs;                                  // [CCS_ID_inputs] дискретные входы (датчик уровня, кнопки и прочее)
        CCS_outputs outputs;                                // [CCS_ID_outputs] выходы контроллера

        // передаваемые по запросу
        uint8_t description[CCS_max_memory_alloc - 1];          // [CCS_ID_description]текстовая строка (ASCIIz) - название контроллера (const для контроллера)
        CCS_config config;                                  // [CCS_ID_config]
        uint8_t fan_channel_assignment[CCS_max_fan_chn];    // [CCS_ID_fan_channel_assignment] перечисление - сколько вентиляторов на каждом канале - по порядку (const для контроллера)
        CCS_termo_power_graph thermo_power_pump_graph;      // [CCS_ID_thermo_power_pump_graph] задание для работы канала при отключенном компьютере.
                                                            // для задания всех графиков эту структуру надо передать CCS_capabilities.num_pumps раз
        CCS_termo_power_graph thermo_power_fan_graph;       // [CCS_ID_thermo_power_fan_graph] задание для работы канала при отключенном компьютере.
                                                            // для задания всех графиков эту структуру надо передать CCS_capabilities.num_fan_channels раз
        CCS_termo_color thermo_color_graph;                 // [CCS_ID_thermo_color_graph] задание подсветки от температуры
        };
        }
        CCS_report_from_controller_to_computer;

        /*==================================================================================*/
        /*
         * структура передаётся компьютером в контроллер
         * тип содержимого определяется полем ID
         * *
        typedef struct {

            uint8_t ID;
        union{
                CCS_config config;
        int8_t pump_channel_power[CCS_max_pump];            // [CCS_ID_pump_channel_power] установки мощности на каналах насосов. 
                                                            // данные настройки переопределяют управление по температуре,
                                                            // но только до обрыва связи с компом.Если значение отрицательное,
                                                            // то вернутья на управление по графику
        uint8_t fan_channel_power[CCS_max_fan_chn];         // [CCS_ID_fan_channel_power] установки мощности на каналах вентиляторов данные. 
                                                            // данные настройки переопределяют управление по температуре,
                                                            // но только до обрыва связи с компом.Если значение отрицательное,
                                                            // то вернутья на управление по графику
        CCS_termo_power_graph thermo_power_pump_graph;      // [CCS_ID_thermo_power_pump_graph] задание для работы канала при отключенном компьютере.
                                                            // для задания всех графиков эту структуру надо передать CCS_capabilities.num_pumps раз
        CCS_termo_power_graph thermo_power_fan_graph;       // [CCS_ID_thermo_power_fan_graph] задание для работы канала при отключенном компьютере.
                                                            // для задания всех графиков эту структуру надо передать CCS_capabilities.num_fan_channels раз
        CCS_termo_color thermo_color_graph;                 // [CCS_ID_thermo_color_graph] задание подсветки от температуры
        CCS_RGB_LED RGB_leds[CCS_max_RGB_led];              // подсветка корпуса RGB
        uint8_t leds[CCS_max_led];                          // светодиоды на плате контроллера
        uint16_t outputs;									// дискретные выходы (реле и прочее )
            };
        }CCS_report_from_computer_to_controller;

        */
    }
}
