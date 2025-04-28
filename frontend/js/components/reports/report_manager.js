export class ReportsManager {
  constructor() {
    this.reportItems = document.querySelectorAll('.report-item');
    this.reportContents = document.querySelectorAll('.report-content');
    
    if (this.reportItems.length && this.reportContents.length) {
      this.init();
    } else {
      console.error('Не найдены элементы отчетов на странице');
    }
  }

  init() {
    this.setupEventListeners();
    if (this.reportItems.length > 0) {
      this.handleReportClick(this.reportItems[0]);
    }
  }

  setupEventListeners() {
    this.reportItems.forEach(item => {
      item.addEventListener('click', (e) => {
        e.preventDefault();
        this.handleReportClick(item);
      });
    });
  }

  handleReportClick(clickedItem) {
    this.reportItems.forEach(i => i.classList.remove('active'));
    this.reportContents.forEach(content => content.classList.remove('active'));

    clickedItem.classList.add('active');

    const reportId = clickedItem.getAttribute('data-report');
    const targetContent = document.getElementById(`report-${reportId}`);
    
    if (targetContent) {
      targetContent.classList.add('active');
    } else {
      console.error(`Не найден контент для отчета ${reportId}`);
    }
  }
}